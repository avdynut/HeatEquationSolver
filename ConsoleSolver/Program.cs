using HeatEquationSolver;

namespace ConsoleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var eq = new Equation(u, k, g, (x, t) => 2 * x * t, (x, t) => 2 * t, (x, t) => x * x, (x, t, u) => 2 * u);

            //EntryPoint.equation = new Equation(QuasiNewton.u, QuasiNewton.K, QuasiNewton.g, QuasiNewton.KDy);
            //EntryPoint.SetUp(HeatEquationSolver.NonlinearSystemSolver.BetaCalculators.MethodBeta.Puzynin);
            //var solver = new Solver();
            //solver.Solve();

            QuasiNewton qn = new QuasiNewton(1, 10, 1, 400, 0.01, 1e-3);
            qn.Solve();
        }

        private static double u(double x, double t)
        {
            return x * x * t;
        }

        private static double k(double x, double t, double u)
        {
            return u * u;
        }

        private static double g(double x, double t, double u)
        {
            return x * x - 8 * x * x * t * t * u - 2 * t * u * u;
        }
    }
}
