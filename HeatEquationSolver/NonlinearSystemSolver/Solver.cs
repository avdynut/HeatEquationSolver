using HeatEquationSolver.NonlinearSystemSolver.BetaCalculators;
using System;
using System.Linq;

namespace HeatEquationSolver.NonlinearSystemSolver
{
    public class Solver
    {
        public int MaxIterations { get; set; }
        private int N;
        private double epsilon;
        private BetaCalculator betaCalculator;

        public Solver(int n, double epsilon, BetaCalculator betaCalculator)
        {
            N = n;
            this.epsilon = epsilon;
            this.betaCalculator = betaCalculator;
        }

        public void Solve(Equation system)
        {
            double[] x = new double[N + 1]; //x = x0;
            double[] f = system.SubstituteValues(x);
            double norm = CalculateNorm(f);
            betaCalculator.Init(norm);
            int iterations = 0;

            while (norm > epsilon)
            {
                MakeSystem();
                var answer = TridiagonalMatrixAlgorithm();

                for (int i = 0; i <= N; i++)
                    x[i] += answer[i];

                f = system.SubstituteValues(x);
                norm = CalculateNorm(f);

                betaCalculator.NextBeta(norm);

                if (++iterations > MaxIterations)
                    throw new Exception("Exceeded max number of iterations");
            }
        }

        private double CalculateNorm(double[] f) // make as extension method
        {
            double sum = f.Sum(x => x * x);
            return Math.Sqrt(sum / N);
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
