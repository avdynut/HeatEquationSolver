using HeatEquationSolver.Settings;
using StringToExpression.LanguageDefinitions;
using System;
using System.Linq.Expressions;

namespace HeatEquationSolver.Equations
{
	public sealed class ParsedEquation : HeatEquation
	{
		public ParsedEquation()
		{
		}

		public ParsedEquation(IFunctions functions)
		{
			var k = Compile(functions.K);
			K = (x, t, u) => (double)k(new Variables { x = x, t = t, u = u });

			var dK_duFunc = Compile(functions.dK_du);
			dK_du = (x, t, u) => (double)dK_duFunc(new Variables { x = x, t = t, u = u });

			var gFunc = Compile(functions.g);
			g = (x, t, u) => (double)gFunc(new Variables { x = x, t = t, u = u, K = K(x, t, u) });

			var initCond = Compile(functions.InitCond);
			InitCond = x => (double)initCond(new Variables { x = x });

			var leftBoundCond = Compile(functions.LeftBoundCond);
			LeftBoundCond = t => (double)leftBoundCond(new Variables { t = t });

			var rightBoundCond = Compile(functions.RightBoundCond);
			RightBoundCond = t => (double)rightBoundCond(new Variables { t = t });

			var uFunc = Compile(functions.u);
			if (uFunc != null)
				u = (x, t) => (double)uFunc(new Variables { x = x, t = t });

			var du_dxFunc = Compile(functions.du_dx);
			if (du_dxFunc != null)
				du_dx = (x, t) => (double)du_dxFunc(new Variables { x = x, t = t });

			var d2u_dx2Func = Compile(functions.d2u_dx2);
			if (d2u_dx2Func != null)
				d2u_dx2 = (x, t) => (double)d2u_dx2Func(new Variables { x = x, t = t });

			var du_dtFunc = Compile(functions.du_dt);
			if (du_dtFunc != null)
				du_dt = (x, t) => (double)du_dtFunc(new Variables { x = x, t = t });
		}

		public Expression<Func<Variables, decimal>> Parse(string func)
		{
			return new ArithmeticLanguage().Parse<Variables>(func);
		}

		private Func<Variables, decimal> Compile(string func)
		{
			return string.IsNullOrEmpty(func) ? null : Parse(func).Compile();
		}

		public struct Variables
		{
			public double x { get; set; }
			public double t { get; set; }
			public double u { get; set; }
			public double K { get; set; }
		}
	}
}
