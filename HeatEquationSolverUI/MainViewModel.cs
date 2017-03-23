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
	public class MainViewModel : ViewModelBase, ISettings
	{
		#region Settings

		private Settings _settings;

		public double X1 { get { return _settings.X1; } set { _settings.X1 = value; } }
		public double X2 { get { return _settings.X2; } set { _settings.X2 = value; OnPropertyChanged(nameof(H)); } }
		public double T1 { get { return _settings.T1; } set { _settings.T1 = value; } }
		public double T2 { get { return _settings.T2; } set { _settings.T2 = value; OnPropertyChanged(nameof(Tau)); } }
		public int N { get { return _settings.N; } set { _settings.N = value; OnPropertyChanged(nameof(H)); } }
		public int M { get { return _settings.M; } set { _settings.M = value; OnPropertyChanged(nameof(Tau)); OnPropertyChanged(nameof(M)); } }
		public double Epsilon { get { return _settings.Epsilon; } set { _settings.Epsilon = value; } }
		public double Epsilon2 { get { return _settings.Epsilon2; } set { _settings.Epsilon2 = value; } }
		public double Alpha { get { return _settings.Alpha; } set { _settings.Alpha = value; } }
		public double Beta0 { get { return _settings.Beta0; } set { _settings.Beta0 = value; } }
		public BetaCalculator BetaCalculatorMethod { get { return _settings.BetaCalculatorMethod; } set { _settings.BetaCalculatorMethod = value; } }
		public int MaxIterations { get { return _settings.MaxIterations; } set { _settings.MaxIterations = value; } }
		public bool UseParsedEquation { get { return _settings.UseParsedEquation; } set { _settings.UseParsedEquation = value; } }
		public Functions Functions { get { return _settings.Functions; } set { _settings.Functions = value; } }

		public double H => _settings.H;
		public double Tau => _settings.Tau;
		public MethodBeta[] MethodsForBeta { get; }
		private MethodBeta currentMethodForBeta;
		public MethodBeta CurrentMethodForBeta
		{
			get { return currentMethodForBeta; }
			set
			{
				currentMethodForBeta = value;
				_settings.BetaCalculatorMethod = currentMethodForBeta.BetaCalculator;
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


		#endregion

		private readonly DelegateCommand solveCommand;
		public ICommand SolveCommand => solveCommand;

		private CancellationTokenSource cancellation;

		public MainViewModel()
		{
			_settings = DataManager.Settings;
			solveCommand = new DelegateCommand(SolveEquation);

			MethodsForBeta = new[] {
				new MethodBeta(BetaCalculator.Puzynin, "Метод Пузынина", ""),
				new MethodBeta(BetaCalculator.No6, "Нерегуляризованный одношаговый метод", ""),
				new MethodBeta(BetaCalculator.No6Mod, "Модифицированный НО метод", "") };
			currentMethodForBeta = MethodsForBeta.First(m => m.BetaCalculator.Equals(_settings.BetaCalculatorMethod));
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

				var qn = new Solver(_settings);

				var progressIndicator = new Progress<int>(ReportProgress);
				var task = Task.Run(() => qn.Solve(cancellation.Token, progressIndicator));
				SolveButtonText = CancelText;
				await task;

				Answer = qn.Answer.Aggregate("", (current, xi) => current + (xi + "\n")).TrimEnd();
				Norm = qn.Norm.ToString();
				DataManager.SaveSettingsToFile();
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
