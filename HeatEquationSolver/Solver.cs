using System;
using System.Linq;
using System.Text;
using HeatEquationSolver.BetaCalculators;
using NLog;
using static HeatEquationSolver.Settings;

namespace HeatEquationSolver
{
    public class Solver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly double h;
        private readonly double tau;
        private readonly double[] x;
        private readonly BetaCalculator betaCalculator;

        public string Answer;
        public double Norm;

        public Solver()
        {
            h = (X2 - X1) / N;
            tau = (T2 - T1) / M;

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
            Logger.Debug("X1={0}, X2={1}, T1={2}, T2={3}, N={4}, M={5}, h={6}, tau={7}, Epsilon={8}, Beta0={9}, MethodForBeta={10}",
                                                                        X1, X2, T1, T2, N, M, h, tau, Epsilon, Beta0, MethodForBeta);
        }

        public void Solve()
        {
            var y = new double[N + 1];
            for (int n = 0; n <= N; n++)
                y[n] = Equation.u(x[n], 0);
            Logger.Debug("Layer 0, y='{0}'", ArrayToString(y));

            double t = T1;
            for (int m = 1; m <= M; m++)
            {
                t += tau;
                y = SolveNonlinearSystem(y, t);
            }

            double sum = 0;
            for (int n = 0; n <= N; n++)
            {
                double sol = Equation.u(x[n], T2);
                Answer += y[n] + "\t" + sol + Environment.NewLine;
                sum += Math.Pow(y[n] - sol, 2);
            }
            Norm = Math.Sqrt(sum);
            Logger.Debug("Answer='{0}', Norm={1}", ArrayToString(y), Norm);
        }

        private double[] SolveNonlinearSystem(double[] y, double t)
        {
            var yK = (double[])y.Clone();
            var f = SubstituteInSystem(t, y, yK);
            double norm = CalculateNorm(f);
            betaCalculator.Init(Beta0, norm);
            int iterations = 0;

            while (norm > Epsilon)
            {
                if (++iterations > MaxIterations)
                    throw new Exception("Exceeded max number of iterations");

                var answer = MakeAndSolveSystem(t, y, yK, f);

                for (int i = 0; i <= N; i++)
                    yK[i] += answer[i];

                f = SubstituteInSystem(t, y, yK);
                norm = CalculateNorm(f);
                betaCalculator.NextBeta(norm);
            }

            Logger.Debug("t={0}, {1} iterations, yK='{2}'", t, iterations, ArrayToString(yK));
            return yK;
        }

        private double[] SubstituteInSystem(double t, double[] y, double[] yK)
        {
            var f = new double[N + 1];
            f[0] = yK[0] - Equation.u(0, t);
            f[N] = yK[N] - Equation.u(X2, t);

            for (int n = 1; n < N; n++)
                f[n] = Equation.dK_dy(x[n], t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                       Equation.K(x[n], t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                       Equation.g(x[n], t, y[n]) - (yK[n] - y[n]) / tau;
            return f;
        }

        private double CalculateNorm(double[] f)
        {
            double sum = f.Sum(e => e * e);
            return Math.Sqrt(sum);  // sum / N
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
                a[n - 1] = l + r;
                b[n - 1] = -l + r;
                c[n] = -2 * r - 1 / tau;
                f[n] *= betaCalculator.Multiplier;
            }
            f[0] *= betaCalculator.Multiplier;
            f[N] *= betaCalculator.Multiplier;

            return ResolvingSystem.TridiagonalMatrixAlgorithm(a, c, b, f);
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
