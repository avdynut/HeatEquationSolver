using HeatEquationSolver.Equations;
using HeatEquationSolver.Settings;
using HeatEquationSolverUI.Base;
using System;
using System.Linq.Expressions;

namespace HeatEquationSolverUI
{
	public class FunctionsViewModel : ViewModelBase, IFunctions
	{
		private IFunctions functions;
		private Settings _settings;

		public string K { get => functions.K; set => functions.K = value; }
		public string dK_du { get => functions.dK_du; set => functions.dK_du = value; }
		public string g { get => functions.g; set => functions.g = value; }

		public string InitCond
		{
			get => functions.InitCond;
			set
			{
				if (string.IsNullOrEmpty(value))
					return;
				functions.InitCond = value;
				OnPropertyChanged(nameof(InitCond));
			}
		}

		public string LeftBoundCond
		{
			get => functions.LeftBoundCond;
			set
			{
				if (string.IsNullOrEmpty(value))
					return;
				functions.LeftBoundCond = value;
				OnPropertyChanged(nameof(LeftBoundCond));
			}
		}

		public string RightBoundCond
		{
			get => functions.RightBoundCond;
			set
			{
				if (string.IsNullOrEmpty(value))
					return;
				functions.RightBoundCond = value;
				OnPropertyChanged(nameof(RightBoundCond));
			}
		}

		public string u
		{
			get => functions.u;
			set
			{
				functions.u = value;
				CalculateInitCond();
				CalculateLeftBoundCond();
				CalculateRightBoundCond();
			}
		}

		public string du_dx { get => functions.du_dx; set => functions.du_dx = value; }
		public string d2u_dx2 { get => functions.d2u_dx2; set => functions.d2u_dx2 = value; }
		public string du_dt { get => functions.du_dt; set => functions.du_dt = value; }

		public FunctionsViewModel(Settings settings)
		{
			_settings = settings;
			functions = settings.Functions;
		}

		public void CalculateInitCond()
		{
			InitCond = SubstituteValueInU("t", _settings.T1.ToString());
		}

		public void CalculateLeftBoundCond()
		{
			LeftBoundCond = SubstituteValueInU("x", _settings.X1.ToString());
		}

		public void CalculateRightBoundCond()
		{
			RightBoundCond = SubstituteValueInU("x", _settings.X2.ToString());
		}

		private string SubstituteValueInU(string symbol, string value)
		{
			var expr = ParseFunction(u);
			if (expr == null)
				return null;
			string func = u.Replace(symbol, value);
			expr = ParseFunction(func);
			return expr?.Body.ToString();
		}

		private Expression<Func<double, double, double>> ParseFunction(string func)
		{
			try
			{
				return new ParsedEquation().Parse(func);
			}
			catch
			{
				return null;
			}
		}
	}
}
