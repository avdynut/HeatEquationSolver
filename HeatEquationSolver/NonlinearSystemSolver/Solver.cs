using System;
using System.Linq;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver.NonlinearSystemSolver
{
    public class Solver
    {
        public int MaxIterations { get; set; } = 50000;

        public double[] Solve(double t, double[] y)
        {
            double[] x = (double[])y.Clone();
            double[] f = equation.SubstituteValues(t, y, x);
            double norm = CalculateNorm(f);
            BetaCalculator.Init(norm);
            int iterations = 0;

            while (norm > Epsilon)
            {
                var answer = MakeAndSolveSystem(t, y, x, f);

                for (int i = 0; i <= N; i++)
                    x[i] += answer[i];

                f = equation.SubstituteValues(t, y, x);
                norm = CalculateNorm(f);

                BetaCalculator.NextBeta(norm);

                //if (++iterations > MaxIterations)
                //    throw new Exception("Exceeded max number of iterations");
            }
            return x;
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
                f[n] *= BetaCalculator.Multiplier;
            }
            f[0] *= BetaCalculator.Multiplier;
            f[N] *= BetaCalculator.Multiplier;

            return TridiagonalMatrixAlgorithm(A, C, B, f);
        }

        private double CalculateNorm(double[] f) // make as extension method
        {
            double sum = f.Sum(x => x * x);
            return Math.Sqrt(sum);
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
