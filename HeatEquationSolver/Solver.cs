using System;
using System.Linq;
using HeatEquationSolver.BetaCalculators;
using NLog;
using static HeatEquationSolver.Settings;

namespace HeatEquationSolver
{
    public class Solver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly double h;
        private readonly double tau;
        private double[] x;
        private BetaCalculator betaCalculator;

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
                    betaCalculator = new PuzyninMethod(Beta0);
                    break;
                case MethodBeta.No6:
                    betaCalculator = new No6Method(Beta0);
                    break;
                case MethodBeta.ModNo6:
                    betaCalculator = new No6ModMethod(Beta0);
                    break;
            }
            //logger.Debug("a={0}, N={1}, T={2}, M={3}, h={4}, tau={5}, beta0={6}, eps={7}", a, N, T, M, h, tau, beta0, eps);
        }

        public void Solve()
        {
            var y = new double[N + 1];
            for (int i = 0; i <= N; i++)
                y[i] = Equation.u(X1 + i * h, 0);

            double t = T1;
            for (int m = 1; m <= M; m++)
            {
                logger.Debug("Layer: {0} -----------------------", m);
                t += tau;
                y = SolveSystem(y, t);
            }

            double sum = 0;
            for (int n = 0; n <= N; n++)
            {
                double sol = Equation.u(x[n], T2);
                Answer += y[n] + "\t" + sol + Environment.NewLine;
                sum += Math.Pow(y[n] - sol, 2);
            }
            Norm = Math.Sqrt(sum);
        }

        double[] SolveSystem(double[] y, double t)
        {
            var f = new double[N + 1];
            var yK = (double[])y.Clone();

            f = fx(t, y, yK);
            double norm = CalculateNorm(f);
            betaCalculator.Init(norm);

            while (norm > Epsilon)
            {
                //logger.Debug("norm = {0}, beta = {1}", norm, beta);

                var answer = MakeAndSolveSystem(t, y, yK, f);

                for (int i = 0; i <= N; i++)
                    yK[i] += answer[i];

                f = fx(t, y, yK);
                norm = CalculateNorm(f);
                betaCalculator.NextBeta(norm);

                //if (++iterations > MaxIterations)
                //    throw new Exception("Exceeded max number of iterations");
            }
            //    for (int n = 0; n <= N; n++)
            //        File.AppendAllText(reportFile, yK[n] + "\t" + u(x[n], t).ToString() + Environment.NewLine);

            return yK;
        }

        private double[] fx(double t, double[] y, double[] yK)
        {
            double[] f = new double[N + 1];
            f[0] = yK[0] - Equation.u(0, t);
            f[N] = yK[N] - Equation.u(X2, t);

            for (int n = 1; n < N; n++)
            {
                f[n] = Equation.dK_dy(x[n], t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                            Equation.K(x[n], t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                            Equation.g(x[n], t, y[n]) - (yK[n] - y[n]) / tau;
            }
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
    }
}
