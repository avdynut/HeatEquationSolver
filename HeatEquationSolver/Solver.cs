using System;
using HeatEquationSolver.NonlinearSystemSolver.BetaCalculators;
using NLog;
using static HeatEquationSolver.Settings;

namespace HeatEquationSolver
{
    public class Solver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly double h;
        private readonly double tau;
        private BetaCalculator betaCalculator;

        public Solver()
        {
            h = (X2 - X1) / N;
            tau = (T2 - T1) / M;

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
        }

        public void Solve()
        {
            var y = new double[N + 1];
            for (int i = 0; i <= N; i++)
                y[i] = equation.u(X1 + i * h, 0);

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
