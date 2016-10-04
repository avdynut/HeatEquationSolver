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
                //MakeSystem();
                //var answer = TridiagonalMatrixAlgorithm();

                //for (int i = 0; i <= N; i++)
                //    x[i] += answer[i];

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
    }
}
