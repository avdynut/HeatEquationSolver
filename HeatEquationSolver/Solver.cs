using System;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver
{
    public class Solver
    {
        public void Solve()
        {
            var y = new double[N + 1];
            for (int i = 0; i <= N; i++)
                y[i] = equation.u(x1 + i * h, 0);

            var solver = new NonlinearSystemSolver.Solver();

            double t = t1;
            for (int m = 1; m <= M; m++)
            {
                logger.Debug("Layer: {0} -----------------------", m);
                t += tau;
                y = solver.Solve(t, y);
            }

            double sum = 0;
            for (int n = 0; n <= N; n++)
            {
                double sol = equation.u(n * h, t2);
                Answer += y[n] + "\t" + sol + Environment.NewLine;
                sum += Math.Pow(y[n] - sol, 2);
            }
            Norm = Math.Sqrt(sum);
        }
    }
}
