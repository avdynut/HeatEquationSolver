using HeatEquationSolver.Settings;
using StringToExpression.LanguageDefinitions;
using System;

namespace HeatEquationSolver.Equations
{
	public class ParsedEquation : HeatEquation
	{
		private readonly ArithmeticLanguage language = new ArithmeticLanguage();

		private Func<Variables, decimal> k;
		public override ComplexFunction K => (x, t, u) => (double)k(new Variables { x = x, t = t, u = u });

		private Func<Variables, decimal> dK_duFunc;
		public override ComplexFunction dK_du => (x, t, u) => (double)dK_duFunc(new Variables { x = x, t = t, u = u });

		private Func<Variables, decimal> gFunc;
		public override ComplexFunction g => (x, t, u) => (double)gFunc(new Variables { x = x, t = t, u = u, K = K(x, t, u) });

		private Func<Variables, decimal> initCond;
		public override InitialCondition InitCond => x => (double)initCond(new Variables { x = x });

		private Func<Variables, decimal> leftBoundCond;
		public override BoundaryCondtion LeftBoundCond => t => (double)leftBoundCond(new Variables { t = t });

		private Func<Variables, decimal> rightBoundCond;
		public override BoundaryCondtion RightBoundCond => t => (double)rightBoundCond(new Variables { t = t });

		#region Optional

		private Func<Variables, decimal> uFunc;
		public override Function u => (x, t) => (double)uFunc(new Variables { x = x, t = t });

		private Func<Variables, decimal> du_dxFunc;
		public override Function du_dx => (x, t) => (double)du_dxFunc(new Variables { x = x, t = t });

		private Func<Variables, decimal> d2u_dx2Func;
		public override Function d2u_dx2 => (x, t) => (double)d2u_dx2Func(new Variables { x = x, t = t });

		private Func<Variables, decimal> du_dtFunc;
		public override Function du_dt => (x, t) => (double)du_dtFunc(new Variables { x = x, t = t });

		#endregion

		public ParsedEquation(IFunctions functions)
		{
			k = Compile(functions.K);
			dK_duFunc = Compile(functions.dK_du);
			gFunc = Compile(functions.g);
			initCond = Compile(functions.InitCond);
			leftBoundCond = Compile(functions.LeftBoundCond);
			rightBoundCond = Compile(functions.RightBoundCond);

			if (!string.IsNullOrEmpty(functions.u))
				uFunc = Compile(functions.u);
			if (!string.IsNullOrEmpty(functions.du_dx))
				du_dxFunc = Compile(functions.du_dx);
			if (!string.IsNullOrEmpty(functions.d2u_dx2))
				d2u_dx2Func = Compile(functions.d2u_dx2);
			if (!string.IsNullOrEmpty(functions.du_dt))
				du_dtFunc = Compile(functions.du_dt);
		}

		private Func<Variables, decimal> Compile(string func)
		{
			return language.Parse<Variables>(func).Compile();
		}

		private struct Variables
		{
			public double x { get; set; }
			public double t { get; set; }
			public double u { get; set; }
			public double K { get; set; }
		}
	}
}
