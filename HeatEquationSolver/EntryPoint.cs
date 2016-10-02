using HeatEquationSolver.NonlinearSystemSolver.BetaCalculators;
using System;

namespace HeatEquationSolver
{
    public static class EntryPoint
    {
        public static int N { get; set; } = 10;
        public static double Epsilon { get; set; } = 1e-5;
        public static double Beta0 { get; set; } = 0.01;        // maybe not here
        public static BetaCalculator BetaCalculator { get; set; }   // maybe not here

        public static void SetUp(MethodBeta methodBeta)
        {
            BetaCalculator betaCalculator = null;
            switch (methodBeta)
            {
                case MethodBeta.Puzynin:
                    betaCalculator = new PuzyninMethod(Beta0, true);
                    break;
                case MethodBeta.No6:
                    betaCalculator = new No6Method(Beta0);
                    break;
                case MethodBeta.ModNo6:
                    betaCalculator = new No6ModMethod(Beta0);
                    break;
                default:
                    new ArgumentException("Incorrect value of method for calculating beta");
                    break;
            }

            var eq = new Equation(u, k, g, (x, t) => 2 * x * t, (x, t) => 2 * t, (x, t) => x * x, (x, t, u) => 2 * u);
            var d = eq.f(4, 3);
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
