using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver
{
    public class Solver
    {
        private int M;
        private double x1;
        private double t1;
        private double h;
        private double tau;
        private double[] y;
        private Equation equation;

        public Solver(double x1, double x2, double t1, double t2, int M, Equation equation)
        {
            h = (x2 - x1) / N;
            tau = (t2 - t1) / M;
            this.M = M;
            this.x1 = x1;
            this.t1 = t1;
            this.equation = equation;
        }

        public void Solve()
        {
            y = new double[N + 1];
            for (int i = 0; i <= N; i++)
                y[i] = equation.u(x1 + i * h, 0);

            double t = t1;
            for (int m = 1; m <= M; m++)
            {
                t += tau;
                Implicit(t);
            }
        }

        private void Implicit(double t)
        {
            double[] A = new double[N + 1];
            double[] C = new double[N + 1];
            double[] B = new double[N + 1];
            double[] f = new double[N + 1];

            C[0] = C[N] = 1;
            f[0] = equation.u(0, t);
            f[N] = equation.u(a, t);
            double x;

            for (int n = 1; n < N; n++)
            {
                x = x1 + n * h;
                A[n] = B[n] = -gamma;
                C[n] = 1 + 2 * gamma;
                f[n] = y[n] + tau * g(x, t);
            }

            y = TridiagonalMatrixAlgorithm(A, C, B, f);
        }
    }
}
