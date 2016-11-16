using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Samples;

namespace HeatEquationSolver
{
    public static class Settings
    {
        /// <summary>
        /// The first 'x' in the scope of the function
        /// </summary>
        public static double X1 { get; set; } = 0;

        /// <summary>
        /// The second 'x' in the scope of the function
        /// </summary>
        public static double X2 { get; set; } = 1;

        /// <summary>
        /// The first 't' in the scope of the function
        /// </summary>
        public static double T1 { get; set; } = 0;

        /// <summary>
        /// The second 't' in the scope of the function
        /// </summary>
        public static double T2 { get; set; } = 1;

        /// <summary>
        /// The number of segments in 'x'
        /// </summary>
        public static int N { get; set; } = 10;

        /// <summary>
        /// The number of segments in 't'
        /// </summary>
        public static int M { get; set; } = 10;

        /// <summary>
        /// Precision in calculating nonlinear systems
        /// </summary>
        public static double Epsilon { get; set; } = 1e-5;

        /// <summary>
        /// Precision in calculating with different steps by 't' axie
        /// </summary>
        public static double Epsilon2 { get; set; } = 1e-4;

        /// <summary>
        /// Multiplier in regularized method
        /// </summary>
        public static double Alpha { get; set; } = 1e-10;

        /// <summary>
        /// The first multiplier 'beta'
        /// </summary>
        public static double Beta0 { get; set; } = 0.01;

        /// <summary>
        /// Method for calculating next beta on each step in iteration process
        /// </summary>
        public static MethodBeta MethodForBeta { get; set; } = MethodBeta.Puzynin;

        /// <summary>
        /// Heat eqaution
        /// </summary>
        public static HeatEquation Equation { get; set; } = new HeatEquation(OldEquation.u, OldEquation.K, OldEquation.g, OldEquation.KDy);

        /// <summary>
        /// Max number of iterations in calculating nonlinear systems
        /// If exceed, then throw Exception
        /// </summary>
        public static int MaxIterations { get; set; } = 50000;
    }
}
