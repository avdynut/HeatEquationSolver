using HeatEquationSolver.NonlinearSystemSolver.BetaCalculators;
using NLog;
using System;

namespace HeatEquationSolver
{
    public static class EntryPoint
    {
        public static double x1 { get; set; } = 0;
        public static double x2 { get; set; } = 1;
        public static double t1 { get; set; } = 0;
        public static double t2 { get; set; } = 1;
        public static double tau { get; set; }
        public static double h { get; set; }
        public static int M { get; set; } = 400;
        public static int N { get; set; } = 10;
        public static double Epsilon { get; set; } = 1e-3;
        public static double Beta0 { get; set; } = 0.01;        // maybe not here
        public static BetaCalculator BetaCalculator { get; set; }   // maybe not here
        public static Equation equation { get; set; }
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static string Answer;
        public static double Norm;

        public static void SetUp(MethodBeta methodBeta)
        {
            tau = (t2 - t1) / M;
            h = (x2 - x1) / N;

            switch (methodBeta)
            {
                case MethodBeta.Puzynin:
                    BetaCalculator = new PuzyninMethod(Beta0, true);
                    break;
                case MethodBeta.No6:
                    BetaCalculator = new No6Method(Beta0);
                    break;
                case MethodBeta.ModNo6:
                    BetaCalculator = new No6ModMethod(Beta0);
                    break;
                default:
                    new ArgumentException("Incorrect value of method for calculating beta");
                    break;
            }
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
