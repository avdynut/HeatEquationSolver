﻿using System;
using NLog;
using System.Linq;
using HeatEquationSolver.NonlinearSystemSolver.BetaCalculators;

namespace HeatEquationSolver
{
    public class QuasiNewton
    {
        int N;
        private int M;
        double a;
        double h;
        double tau;
        double eps;
        private double beta0;
        public string Answer;
        public double Norm;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private BetaCalculator betaCalculator;
        private Equation equation;

        public QuasiNewton(double a, int N, double T, int M, double beta0, double eps, Equation equation)
        {
            this.a = a;
            this.N = N;
            h = a / N;
            tau = T / M;
            this.M = M;
            this.beta0 = beta0;
            this.eps = eps;
            this.equation = equation;
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
            betaCalculator = new PuzyninMethod(beta0);
            betaCalculator.Init(norm);

            while (norm > eps)
            {
                //logger.Debug("norm = {0}, beta = {1}", norm, beta);

                var answer = MakeAndSolveSystem(t, y, yK, f);

                for (int n = 0; n <= N; n++)
                    yK[n] += answer[n];

                f = fx(t, y, yK);
                norm = CalculateNorm(f);
                betaCalculator.NextBeta(norm);
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

        private double[] MakeAndSolveSystem(double t, double[] y, double[] yK, double[] f)
        {
            double[] A = new double[N + 1];
            double[] C = new double[N + 1];
            double[] B = new double[N + 1];
            double x, l, r;
            C[0] = C[N] = 1;

            for (int n = 1; n < N; n++)
            {
                x = n * h;
                l = equation.dK_dy(x, t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
                r = equation.K(x, t, y[n]) / (h * h);
                A[n - 1] = l + r;
                B[n - 1] = -l + r;
                C[n] = -2 * r - 1 / tau;
                //r = -K(x, t, y[n]) / (h * h);
                //A[n - 1] = -l + r;
                //B[n - 1] = l + r;
                //C[n] = -2 * r + 1 / tau;
                f[n] *= betaCalculator.Multiplier;
            }
            f[0] *= betaCalculator.Multiplier;
            f[N] *= betaCalculator.Multiplier;

            return TridiagonalMatrixAlgorithm(A, C, B, f);
        }

        private double[] TridiagonalMatrixAlgorithm(double[] a, double[] c, double[] b, double[] f)
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
