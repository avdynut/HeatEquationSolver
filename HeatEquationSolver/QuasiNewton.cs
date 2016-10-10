using System;
using NLog;
using System.Linq;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver
{
    public class QuasiNewton
    {
        int N;
        private int M;
        double a;
        double tau;
        double eps;
        private double beta0;
        public string Answer;
        public double Norm;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private SystemWorker.SystemWorker worker;

        public QuasiNewton(double a, int N, double T, int M, double beta0, double eps, SystemWorker.SystemWorker worker)
        {
            this.a = a;
            this.N = N;
            tau = T / M;
            this.M = M;
            this.beta0 = beta0;
            this.eps = eps;
            this.worker = worker;
            logger.Debug("a={0}, N={1}, T={2}, M={3}, h={4}, tau={5}, beta0={6}, eps={7}", a, N, T, M, h, tau, beta0, eps);
        }

        public void Solve()
        {
            var y = new double[N + 1];     // mu1 = u(x, 0)
            for (int i = 0; i <= N; i++)
                y[i] = equation.u(i * h, 0);

            for (int m = 1; m <= M; m++)
            {
                logger.Debug("Layer: {0} -----------------------", m);

                double t = m * tau;
                y = SolveSystem(y, t);
            }

            double sum = 0;
            double T = tau * M;
            for (int n = 0; n <= N; n++)
            {
                double sol = equation.u(n * h, T);
                Answer += y[n] + "\t" + sol + Environment.NewLine;
                sum += Math.Pow(y[n] - sol, 2);
            }
            Norm = Math.Sqrt(sum);
        }

        double[] SolveSystem(double[] y, double t)
        {
            double[] f = new double[N + 1];
            double[] yK = (double[])y.Clone();

            f = fx(t, y, yK);
            double norm = CalculateNorm(f);
            BetaCalculator = new NonlinearSystemSolver.BetaCalculators.PuzyninMethod(beta0);
            BetaCalculator.Init(norm);

            while (norm > eps)
            {
                //logger.Debug("norm = {0}, beta = {1}", norm, beta);

                var answer = worker.MakeAndSolveSystem(t, y, yK, f);

                for (int n = 0; n <= N; n++)
                    yK[n] += answer[n];

                f = fx(t, y, yK);
                norm = CalculateNorm(f);
                BetaCalculator.NextBeta(norm);
            }
            //    for (int n = 0; n <= N; n++)
            //        File.AppendAllText(reportFile, yK[n] + "\t" + u(n * h, t).ToString() + Environment.NewLine);

            return yK;
        }

        private double[] fx(double t, double[] y, double[] yK)
        {
            double[] f = new double[N + 1];
            double x;
            f[0] = yK[0] - equation.u(0, t);
            f[N] = yK[N] - equation.u(a, t);
            for (int n = 1; n < N; n++)
            {
                x = n * h;
                f[n] = equation.dK_dy(x, t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                            equation.K(x, t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                            equation.g(x, t, y[n]) - (yK[n] - y[n]) / tau;
                //f[n] = (yK[n] - y[n]) / tau
                //     - KDy(x, t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2)
                //     - K(x, t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h)
                //     - g(x, t, y[n]);
            }
            return f;
        }

        private double CalculateNorm(double[] f)
        {
            double sum = f.Sum(x => x * x);
            return Math.Sqrt(sum);  // sum / N
        }
    }
}
