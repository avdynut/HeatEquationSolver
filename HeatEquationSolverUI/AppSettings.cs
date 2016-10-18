using HeatEquationSolver;
using HeatEquationSolver.BetaCalculators;

namespace HeatEquationSolverUI
{
    public class AppSettings
    {
        public double X1 { get { return Settings.X1; } set { Settings.X1 = value; } }
        public double X2 { get { return Settings.X2; } set { Settings.X2 = value; } }
        public double T1 { get { return Settings.T1; } set { Settings.T1 = value; } }
        public double T2 { get { return Settings.T2; } set { Settings.T2 = value; } }
        public int N { get { return Settings.N; } set { Settings.N = value; } }
        public int M { get { return Settings.M; } set { Settings.M = value; } }
        public double Epsilon { get { return Settings.Epsilon; } set { Settings.Epsilon = value; } }
        public double Alpha { get { return Settings.Alpha; } set { Settings.Alpha = value; } }
        public double Beta0 { get { return Settings.Beta0; } set { Settings.Beta0 = value; } }
        public MethodBeta MethodForBeta { get { return Settings.MethodForBeta; } set { Settings.MethodForBeta = value; } }
        public HeatEquation Equation { get { return Settings.Equation; } set { Settings.Equation = value; } }
        public int MaxIterations { get { return Settings.MaxIterations; } set { Settings.MaxIterations = value; } }
    }
}
