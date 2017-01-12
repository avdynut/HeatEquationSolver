using HeatEquationSolver.BetaCalculators;
using NLog;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using static HeatEquationSolver.Settings;

namespace HeatEquationSolver
{
    public class Solver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly double h;
        private readonly double Tau;
        private readonly double[] x;
        private readonly BetaCalculator betaCalculator;

        public string Answer;
        public double Norm;
        private double tau;

        public Solver()
        {
            h = H;
			Tau = Settings.Tau;

            x = new double[N + 1];
            for (int i = 0; i <= N; i++)
                x[i] = X1 + i * h;

            switch (MethodForBeta)
            {
                case MethodBeta.Puzynin:
                    betaCalculator = new PuzyninMethod();
                    break;
                case MethodBeta.No6:
                    betaCalculator = new No6Method();
                    break;
                case MethodBeta.ModNo6:
                    betaCalculator = new No6ModMethod();
                    break;
            }
            Logger.Debug("X1={0}, X2={1}, T1={2}, T2={3}, N={4}, M={5}, h={6}, Tau={7}, Epsilon={8}, Beta0={9}, MethodForBeta={10}",
                                                                        X1, X2, T1, T2, N, M, h, Tau, Epsilon, Beta0, MethodForBeta);
        }

        public void Solve(CancellationToken cancellationToken, IProgress<int> progress = null)
        {
            var y0 = new double[N + 1];
            for (int n = 0; n <= N; n++)
                y0[n] = Equation.InitCond(x[n]);
            Logger.Debug("Layer 0, y='{0}'", ArrayToString(y0));

            for (int m = 0; m < M; m++)
            {
                progress?.Report(m);
                tau = Tau;
                var yWithPredTau = SolveNonlinearSystem(y0, T1 + m * Tau + Tau);
                int k = 1;
                double norm;
                double[] yWithNextTau;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    k *= 2;
                    tau = Tau / k;
                    double t0 = T1 + m * Tau;
                    yWithNextTau = (double[])y0.Clone();
                    for (int i = 0; i < k; i++)
                        yWithNextTau = SolveNonlinearSystem(yWithNextTau, t0 += tau);
                    norm = CalculateNorm(yWithPredTau, yWithNextTau);
                    Logger.Debug("norm={0}", norm);
                    yWithPredTau = (double[])yWithNextTau.Clone();

                } while (norm > Epsilon2);
                y0 = yWithNextTau;
                Logger.Debug("m={0}, tau=Tau/{1}, norm={2}", m, k, norm);
            }

            double sum = 0;
            for (int n = 0; n <= N; n++)
            {
                double sol = Equation.u(x[n], T2);
                Answer += y0[n] + "\t" + sol + Environment.NewLine;
                sum += Math.Pow(y0[n] - sol, 2);
            }
            Norm = Math.Sqrt(sum);
            Logger.Debug("Answer='{0}', Norm={1}", ArrayToString(y0), Norm);
        }

        private double[] SolveNonlinearSystem(double[] y, double t)
        {
            var yK = (double[])y.Clone();
            var f = SubstituteInSystem(t, y, yK);
            Norm = CalculateNorm(f);
            betaCalculator.Init(Beta0, Norm);
            int iterations = 0;

            while (Norm > Epsilon)
            {
                if (++iterations > MaxIterations)
                    throw new Exception("Exceeded max number of iterations");

                var answer = ReqularizedMethod(t, y, yK, (double[])f.Clone());

                for (int i = 0; i <= N; i++)
                    yK[i] += answer[i];

                f = SubstituteInSystem(t, y, yK);
                Norm = CalculateNorm(f);
                betaCalculator.NextBeta(Norm);
            }

            Logger.Debug("t={0}, {1} iterations, yK='{2}'", t, iterations, ArrayToString(yK));
            return yK;
        }

        private double[] SubstituteInSystem(double t, double[] y, double[] yK)
        {
            var f = new double[N + 1];
            f[0] = yK[0] - Equation.LeftBoundCond(t);
            f[N] = yK[N] - Equation.RightBoundCond(t);

            for (int n = 1; n < N; n++)
                f[n] = Equation.dK_dy(x[n], t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                       Equation.K(x[n], t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                       Equation.g(x[n], t, y[n]) - (yK[n] - y[n]) / tau;
            return f;
        }

        private double CalculateNorm(double[] f)
        {
            double sum = f.Sum(e => e * e);
            return Math.Sqrt(sum / N);
        }

        private double CalculateNorm(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
                sum += Math.Pow(a[i] - b[i], 2);
            return Math.Sqrt(sum / N);
        }

        private double[] MakeAndSolveSystem(double t, double[] y, double[] yK, double[] f)
        {
            var a = new double[N + 1];
            var c = new double[N + 1];
            var b = new double[N + 1];
            c[0] = c[N] = 1;

            for (int n = 1; n < N; n++)
            {
                double l = Equation.dK_dy(x[n], t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
                double r = Equation.K(x[n], t, y[n]) / (h * h);
                a[n - 1] = l + r;       // or a[n],b[n]?
                b[n - 1] = -l + r;
                c[n] = -2 * r - 1 / tau;
                f[n] *= betaCalculator.Multiplier;
            }
            f[0] *= betaCalculator.Multiplier;
            f[N] *= betaCalculator.Multiplier;

            return ResolvingSystem.TridiagonalMatrixAlgorithm(a, c, b, f);
        }

        private double[] FullMatrix(double t, double[] y, double[] yK, double[] f)
        {
            double alphaBetaNorm = Alpha * betaCalculator.Beta * Norm;
            var jacobian = new double[N + 1, N + 1];     // f'(Xn)
            jacobian[0, 0] = jacobian[N, N] = 1;
            for (int n = 1; n < N; n++)
            {
                double l = Equation.dK_dy(x[n], t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
                double r = Equation.K(x[n], t, y[n]) / (h * h);
                jacobian[n, n - 1] = l + r;
                jacobian[n, n + 1] = -l + r;
                jacobian[n, n] = -2 * r - 1 / tau;
                f[n] *= betaCalculator.Multiplier;
            }
            f[0] *= betaCalculator.Multiplier;
            f[N] *= betaCalculator.Multiplier;
            return ResolvingSystem.Gauss(jacobian, f);
        }

        private double[] ReqularizedMethod(double t, double[] y, double[] yK, double[] f)
        {
            double alphaBetaNorm = Alpha * betaCalculator.Beta * Norm;
            var jacobian = new double[N + 1, N + 1];     // f'(Xn)
            jacobian[0, 0] = jacobian[N, N] = 1;
            for (int n = 1; n < N; n++)
            {
                double l = Equation.dK_dy(x[n], t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
                double r = Equation.K(x[n], t, y[n]) / (h * h);
                jacobian[n, n - 1] = l + r;
                jacobian[n, n + 1] = -l + r;
                jacobian[n, n] = -2 * r - 1 / tau;
            }

            var a = Matrix.Transpose(jacobian);
            a = Matrix.AddDiag(a, alphaBetaNorm);
            var matrix = Matrix.AddDiag(Matrix.Multiply(a, jacobian), alphaBetaNorm);
            var freeMembers = Vector.MultiplyConst(betaCalculator.Multiplier, Matrix.Multiply(a, f));
            return ResolvingSystem.Gauss(matrix, freeMembers);
        }

        private string ArrayToString(double[] array)
        {
            var sb = new StringBuilder();
            foreach (double t in array)
            {
                sb.Append(t);
                sb.Append("; ");
            }
            return sb.ToString();
        }
    }
}
