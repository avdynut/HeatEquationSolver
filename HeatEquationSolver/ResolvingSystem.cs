using System;
using System.Linq;

namespace HeatEquationSolver
{
    public static class ResolvingSystem
    {
        public static double[] Gauss(double[,] system)
        {
            double[,] m = (double[,]) system.Clone();
            int n = m.GetLength(0);
            for (int k = 0; k < n - 1; k++)
                for (int i = k + 1; i < n; i++)
                {
                    double koef = -m[i, k]/m[k, k];
                    for (int j = 0; j < n + 1; j++)
                        m[i, j] += m[k, j]*koef;
                }

            var deltaX = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i; j < n; j++)
                    sum += deltaX[j]*m[i, j];
                deltaX[i] = (m[i, n] - sum)/m[i, i];
            }

            if (deltaX.Any(x => double.IsInfinity(x) || double.IsNaN(x)))
                throw new ArithmeticException("System is not have solution");

            return deltaX;
        }

        public static double[] Gauss(double[,] coefficients, double[] freeMembers)
        {
            int n = coefficients.GetLength(0);
            if (coefficients.GetLength(1) < n || freeMembers.Length < n)
                throw new ArgumentException("System should be nxn");

            var system = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    system[i, j] = coefficients[i, j];
                system[i, n] = freeMembers[i];
            }

            return Gauss(system);
        }

        public static double[] TridiagonalMatrixAlgorithm(double[] a, double[] c, double[] b, double[] f)
        {
            int n = c.Length;
            var x = new double[n];

            for (int i = 1; i < n; i++)
            {
                double m = a[i] / c[i - 1];
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
