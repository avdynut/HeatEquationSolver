using HeatEquationSolver;
using HeatEquationSolver.BetaCalculators;
using HeatEquationSolverUI.Base;

namespace HeatEquationSolverUI
{
    public class SolverSettings : ViewModelBase
    {
        public double X1 { get { return Settings.X1; } set { Settings.X1 = value; } }
        public double X2 { get { return Settings.X2; } set { Settings.X2 = value; OnPropertyChanged(nameof(H)); } }
        public double T1 { get { return Settings.T1; } set { Settings.T1 = value; } }
        public double T2 { get { return Settings.T2; } set { Settings.T2 = value; OnPropertyChanged(nameof(Tau)); } }
        public int N { get { return Settings.N; } set { Settings.N = value; OnPropertyChanged(nameof(H)); } }
        public int M { get { return Settings.M; } set { Settings.M = value; OnPropertyChanged(nameof(Tau)); OnPropertyChanged(nameof(M)); } }
        public double H { get { return Settings.H; } }
        public double Tau { get { return Settings.Tau; } }
        public double Epsilon { get { return Settings.Epsilon; } set { Settings.Epsilon = value; } }
        public double Epsilon2 { get { return Settings.Epsilon2; } set { Settings.Epsilon2 = value; } }
        public double Alpha { get { return Settings.Alpha; } set { Settings.Alpha = value; } }
        public double Beta0 { get { return Settings.Beta0; } set { Settings.Beta0 = value; } }
        public HeatEquation Equation { get { return Settings.Equation; } set { Settings.Equation = value; } }
        public int MaxIterations { get { return Settings.MaxIterations; } set { Settings.MaxIterations = value; } }

        public MethodBeta[] MethodsForBeta { get; }
        private MethodBeta currentMethodForBeta;
        public MethodBeta CurrentMethodForBeta
        {
            get { return currentMethodForBeta; }
            set
            {
                currentMethodForBeta = value;
                Settings.BetaCalculator = currentMethodForBeta.BetaCalculator;
            }
        }

        public SolverSettings()
        {
            MethodsForBeta = new MethodBeta[] {
                new MethodBeta(new PuzyninMethod(), "Метод Пузынина", ""),
                new MethodBeta(new No6Method(), "Нерегуляризованный одношаговый метод", ""),
                new MethodBeta(new No6ModMethod(), "Модифицированный НО метод", "") };
            currentMethodForBeta = MethodsForBeta[0];
        }
    }
}
