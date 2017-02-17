using HeatEquationSolver;
using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Equations;
using HeatEquationSolverUI.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HeatEquationSolverUI
{
    public class MainViewModel : ViewModelBase
    {
        #region Settings

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
        //public int MaxIterations { get { return Settings.MaxIterations; } set { Settings.MaxIterations = value; } }

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

        #endregion

        #region UI controls

        private const string SolveText = "Решить";
        private const string CancelText = "Отмена";
        private string solveButtonText = SolveText;
        public string SolveButtonText
        {
            get { return solveButtonText; }
            set { solveButtonText = value; OnPropertyChanged(nameof(SolveButtonText)); }
        }

        private bool solveButtonIsEnabled = true;
        public bool SolveButtonIsEnabled
        {
            get { return solveButtonIsEnabled; }
            set { solveButtonIsEnabled = value; OnPropertyChanged(nameof(SolveButtonIsEnabled)); }
        }

        private string norm;
        public string Norm
        {
            get { return norm; }
            set { norm = value; OnPropertyChanged(nameof(Norm)); }
        }

        private string answer;
        public string Answer
        {
            get { return answer; }
            set { answer = value; OnPropertyChanged(nameof(Answer)); }
        }

        public int ProgressBarValue { get; set; }
        public bool UseOwnEq { get; set; } = true;
        public Equation Equation { get; } = new Equation();

        #endregion

        private readonly DelegateCommand solveCommand;
        public ICommand SolveCommand => solveCommand;

        private CancellationTokenSource cancellation;

        public MainViewModel()
        {
            solveCommand = new DelegateCommand(SolveEquation);

            MethodsForBeta = new MethodBeta[] {
                new MethodBeta(new PuzyninMethod(), "Метод Пузынина", ""),
                new MethodBeta(new No6Method(), "Нерегуляризованный одношаговый метод", ""),
                new MethodBeta(new No6ModMethod(), "Модифицированный НО метод", "") };
            currentMethodForBeta = MethodsForBeta[0];
        }

        private async void SolveEquation()
        {
            if (SolveButtonText == CancelText)
            {
                SolveButtonIsEnabled = false;
                cancellation.Cancel();
                return;
            }

            cancellation = new CancellationTokenSource();
            try
            {
                Norm = Answer = string.Empty;
                if (UseOwnEq)
                {
                    Equation.ParsedEq.ParseFunctions();
                    Settings.Equation = Equation.ParsedEq;
                }
                else
                    Settings.Equation = new ModelEquation();

                var qn = new Solver();

                var progressIndicator = new Progress<int>(ReportProgress);
                var task = Task.Run(() => qn.Solve(cancellation.Token, progressIndicator));
                SolveButtonText = CancelText;
                await task;

                Answer = qn.Answer.Aggregate("", (current, xi) => current + (xi + "\n")).TrimEnd();
                Norm = qn.Norm.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SolveButtonText = SolveText;
            SolveButtonIsEnabled = true;
        }

        private void ReportProgress(int value)
        {
            ProgressBarValue = ++value;
            OnPropertyChanged(nameof(ProgressBarValue));
        }
    }
}
