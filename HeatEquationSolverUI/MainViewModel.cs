using HeatEquationSolver;
using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Settings;
using HeatEquationSolverUI.Base;
using System;
using System.Diagnostics;
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

		private readonly Settings _settings;

		public double X1
		{
			get => _settings.X1;
			set
			{
				_settings.X1 = value;
				((FunctionsViewModel)Functions).CalculateLeftBoundCond();
			}
		}

		public double X2
		{
			get => _settings.X2;
			set
			{
				_settings.X2 = value;
				((FunctionsViewModel)Functions).CalculateRightBoundCond();
				OnPropertyChanged(nameof(H));
			}
		}

		public double T1
		{
			get => _settings.T1;
			set
			{
				_settings.T1 = value;
				((FunctionsViewModel)Functions).CalculateInitCond();
			}
		}
		public double T2
		{
			get => _settings.T2;
			set { _settings.T2 = value; OnPropertyChanged(nameof(Tau)); }
		}

		public int N
		{
			get => _settings.N;
			set { _settings.N = value; OnPropertyChanged(nameof(H)); }
		}

		public int M
		{
			get => _settings.M;
			set
			{
				_settings.M = value;
				OnPropertyChanged(nameof(Tau));
				OnPropertyChanged(nameof(M));
				ProgressBarValue = 0;
			}
		}

		public double Epsilon { get => _settings.Epsilon; set => _settings.Epsilon = value; }
		public double Epsilon2 { get => _settings.Epsilon2; set => _settings.Epsilon2 = value; }
		public double Alpha { get => _settings.Alpha; set => _settings.Alpha = value; }
		public double Beta0 { get => _settings.Beta0; set => _settings.Beta0 = value; }
		public BetaCalculator BetaCalculatorMethod { get => _settings.BetaCalculatorMethod; set => _settings.BetaCalculatorMethod = value; }
		public int MaxIterations { get => _settings.MaxIterations; set => _settings.MaxIterations = value; }
		public bool UseParsedEquation { get => _settings.UseParsedEquation; set => _settings.UseParsedEquation = value; }
		public IFunctions Functions { get; set; }
		public double H => _settings.H;
		public double Tau => _settings.Tau;
		private MethodBeta _currentMethodForBeta;
		public MethodBeta[] MethodsForBeta => MethodBeta.Methods;

		public MethodBeta CurrentMethodForBeta
		{
			get => _currentMethodForBeta;
			set
			{
				_currentMethodForBeta = value;
				OnPropertyChanged(nameof(CurrentMethodForBeta));
				_settings.BetaCalculatorMethod = _currentMethodForBeta.BetaCalculator;
			}
		}

		#endregion

		#region UI controls

		private const string SolveText = "Решить";
		private const string CancelText = "Отмена";
		private string _solveButtonText = SolveText;
		public string SolveButtonText
		{
			get => _solveButtonText;
			set { _solveButtonText = value; OnPropertyChanged(nameof(SolveButtonText)); }
		}

		private bool _solveButtonIsEnabled = true;
		public bool SolveButtonIsEnabled
		{
			get => _solveButtonIsEnabled;
			set { _solveButtonIsEnabled = value; OnPropertyChanged(nameof(SolveButtonIsEnabled)); }
		}

		private string _norm;
		public string Norm
		{
			get => _norm;
			set { _norm = value; OnPropertyChanged(nameof(Norm)); }
		}

		private string _answer;
		public string Answer
		{
			get => _answer;
			set { _answer = value; OnPropertyChanged(nameof(Answer)); }
		}

		private int _progressValue;
		public int ProgressBarValue
		{
			get => _progressValue;
			set { _progressValue = value; OnPropertyChanged(nameof(ProgressBarValue)); }
		}

		public double ElapsedSeconds { get; set; }

		#endregion

		private readonly DelegateCommand _solveCommand;
		public ICommand SolveCommand => _solveCommand;
		private CancellationTokenSource _cancellation;

		public MainViewModel()
		{
			_settings = DataManager.Settings;
			Functions = new FunctionsViewModel(_settings);
			_solveCommand = new DelegateCommand(SolveEquation);
			_currentMethodForBeta = MethodBeta.Methods.First(m => m.BetaCalculator.Equals(_settings.BetaCalculatorMethod));
		}

		private async void SolveEquation()
		{
			if (SolveButtonText == CancelText)
			{
				SolveButtonIsEnabled = false;
				_cancellation.Cancel();
				return;
			}

			_cancellation = new CancellationTokenSource();
			try
			{
				Norm = Answer = string.Empty;

				var qn = new Solver(_settings);

				var progressIndicator = new Progress<int>(ReportProgress);
				var task = Task.Run(() => qn.Solve(_cancellation.Token, progressIndicator));
				SolveButtonText = CancelText;
				var sw = new Stopwatch();
				sw.Start();
				await task;
				sw.Stop();
				ElapsedSeconds = sw.Elapsed.TotalSeconds;
				OnPropertyChanged(nameof(ElapsedSeconds));

				Answer = string.Join(Environment.NewLine, qn.Answer);
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
		}
	}
}
