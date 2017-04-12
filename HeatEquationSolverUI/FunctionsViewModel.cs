using HeatEquationSolver.Equations;
using HeatEquationSolver.Settings;
using HeatEquationSolverUI.Base;
using System;
using System.Linq.Expressions;

namespace HeatEquationSolverUI
{
	public class FunctionsViewModel : ViewModelBase, IFunctions
	{
		private readonly ParsedEquation parsedEquation;
		private Settings _settings;
		private IFunctions functions;

		public string K
		{
			get => functions.K;
			set
			{
				functions.K = value;
				var expr = ParseFunction<Func<double, double, double, double>>(K, "x", "t", "u");
				if (expr == null)
					return;
				dK_du = parsedEquation.Derivative(expr, "u").Body.ToString().Replace(" ", "");
				CalculateG();
			}
		}

		public string dK_du
		{
			get => functions.dK_du;
			set
			{
				functions.dK_du = value;
				OnPropertyChanged(nameof(dK_du));
			}
		}

		public string g
		{
			get => functions.g;
			set
			{
				functions.g = value;
				OnPropertyChanged(nameof(g));
			}
		}

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
				CalculateG();
			}
		}

		public string du_dx { get => functions.du_dx; set => functions.du_dx = value; }
		public string d2u_dx2 { get => functions.d2u_dx2; set => functions.d2u_dx2 = value; }
		public string du_dt { get => functions.du_dt; set => functions.du_dt = value; }

		public FunctionsViewModel(Settings settings)
		{
			parsedEquation = new ParsedEquation();
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
			var expr = ParseFunction<Func<double, double, double>>(u, "x", "t");
			if (expr == null)
				return null;
			string func = u.Replace(symbol, value);
			expr = ParseFunction<Func<double, double, double>>(func, "x", "t");
			return expr?.Body.ToString().Replace(" ", "");
		}

		public void CalculateG()
		{
			var exprU = ParseFunction<Func<double, double, double>>(u, "x", "t");
			var exprK = ParseFunction<Func<double, double, double, double>>(K, "x", "t", "u");
			if (exprU == null || exprK == null)
				return;
			du_dt = parsedEquation.Derivative(exprU, "t").Body.ToString();
			var dUdx = parsedEquation.Derivative(exprU, "x");
			du_dx = dUdx.Body.ToString();
			d2u_dx2 = parsedEquation.Derivative(dUdx, "x").Body.ToString();
			g = $"{du_dt}-{dK_du}*Math.Pow({du_dx},2)-{d2u_dx2}*K".Replace(" ", "");
		}

		private Expression<T> ParseFunction<T>(string func, params string[] variables)
		{
			try
			{
				return parsedEquation.Parse<T>(func, variables);
			}
			catch
			{
				return null;
			}
		}
	}
}
