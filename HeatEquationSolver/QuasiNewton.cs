using System;
using NLog;

namespace HeatEquationSolver
{
    public class QuasiNewton
    {
        int N;                                  // количество разбиений по x
        double a;                               // длина отрезка по x
        double h;                               // шаг по x
        double tau;                             // шаг по t
        double eps;                             // точность
        public string Answer;
        public double Norm;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public QuasiNewton(double a, int N, double T, int M, double beta0, double eps)
        {
            this.a = a;
            this.N = N;
            h = a / N;
            tau = T / M;
            this.eps = eps;

            logger.Debug("a={0}, N={1}, T={2}, M={3}, h={4}, tau={5}, beta0={6}, eps={7}", a, N, T, M, h, tau, beta0, eps);

            var y = new double[N + 1];     // mu1 = u(x, 0)
            for (int i = 0; i <= N; i++)
                y[i] = u(i * h, 0);

            for (int m = 1; m <= M; m++)
            {
                logger.Debug("Слой: {0} -----------------------", m);

                double t = m * tau;
                y = SolveSystem(y, t, beta0);
            }

            double sum = 0;
            for (int n = 0; n <= N; n++)
            {
                double sol = u(n * h, T);
                Answer += y[n] + "\t" + sol + Environment.NewLine;
                sum += Math.Pow(y[n] - sol, 2);
            }
            Norm = Math.Sqrt(sum);
        }

        double normDifference(double[] x, double[] y)
        {
            double sum = 0;
            for (int n = 0; n <= N; n++)
                sum += Math.Pow(x[n] - y[n], 2);
            sum /= N;
            return Math.Sqrt(sum);
        }

        double[] SolveSystem(double[] y, double t, double beta)
        {
            double[] A = new double[N + 1];
            double[] C = new double[N + 1];
            double[] B = new double[N + 1];
            double[] f = new double[N + 1];
            double[] yK = (double[])y.Clone();

            C[0] = C[N] = 1;
            f = fx(t, y, yK);
            double norm = getNorm(f);

            while (norm > eps)
            {
                logger.Debug("norm = {0}, beta = {1}", norm, beta);
                double x, l, r;

                for (int n = 1; n < N; n++)
                {
                    x = n * h;
                    l = KDu(x, t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
                    r = K(x, t, y[n]) / (h * h);
                    A[n - 1] = l + r;
                    B[n - 1] = -l + r;
                    C[n] = -2 * r - 1 / tau;
                    f[n] *= -beta;
                }
                f[0] *= -beta;
                f[N] *= -beta;

                double[] sol = TridiagonalMatrixAlgorithm(A, C, B, f);

                for (int n = 0; n <= N; n++)
                    yK[n] += sol[n];

                f = fx(t, y, yK);
                double newNorm = getNorm(f);
                double b = beta * norm / newNorm;
                if (b > beta)
                    beta = (b < 1) ? b : 1;
                norm = newNorm;
            }
            //    for (int n = 0; n <= N; n++)
            //        File.AppendAllText(reportFile, yK[n] + "\t" + u(n * h, t).ToString() + Environment.NewLine);

            y = (double[])yK.Clone();
            return y;
        }

        double[] fx(double t, double[] y, double[] yK)
        {
            double[] f = new double[N + 1];
            double x;
            f[0] = yK[0] - u(0, t);
            f[N] = yK[N] - u(a, t);
            for (int n = 1; n < N; n++)
            {
                x = n * h;
                f[n] = KDu(x, t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                            K(x, t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                            g(x, t, y[n]) - (yK[n] - y[n]) / tau;
            }
            return f;
        }

        double getNorm(double[] f)
        {
            double sum = 0;
            for (int n = 0; n <= N; n++)
                sum += f[n] * f[n];
            return Math.Sqrt(sum);
        }

        double u(double x, double t)
        {
            return x * x * t + 3 * x * t * t;
        }

        double g(double x, double t, double y)
        {
            return x * x + 6 * t * x - 2 * y * Math.Pow(2 * x * t + 3 * t * t, 2) - 2 * t * (x * x * t + y * y);
        }

        double K(double x, double t, double y)
        {
            return x * x * t + y * y;
        }

        double KDu(double x, double t, double y)
        {
            return 2 * y;
        }

        public double[] TridiagonalMatrixAlgorithm(double[] a, double[] c, double[] b, double[] f)
        {
            int n = c.Length;
            var x = new double[n];
            double m;

            for (int i = 1; i < n; i++)
            {
                m = a[i] / c[i - 1];
                c[i] -= m * b[i - 1];
                f[i] -= m * f[i - 1];
            }

            x[n - 1] = f[n - 1] / c[n - 1];
            for (int i = n - 2; i >= 0; i--)
                x[i] = (f[i] - b[i] * x[i + 1]) / c[i];

            return x;
        }
    }
}
