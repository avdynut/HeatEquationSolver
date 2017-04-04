using Mathos.Parser;

namespace HeatEquationSolver.Equations
{
	public class ParsedEquation : HeatEquation
	{
		private MathParser parser = new MathParser();

		private ComplexFunction k;
		public override ComplexFunction K => k;

		private ComplexFunction dK_duFunc;
		public override ComplexFunction dK_du => dK_duFunc;

		private ComplexFunction gFunc;
		public override ComplexFunction g => gFunc;

		private InitialCondition initCond;
		public override InitialCondition InitCond => initCond;

		private BoundaryCondtion leftBoundCond;
		public override BoundaryCondtion LeftBoundCond => leftBoundCond;

		private BoundaryCondtion rightBoundCond;
		public override BoundaryCondtion RightBoundCond => rightBoundCond;

		#region Optional

		private Function uFunc;
		public override Function u => uFunc;

		private Function du_dxFunc;
		public override Function du_dx => du_dxFunc;

		private Function d2u_dx2Func;
		public override Function d2u_dx2 => d2u_dx2Func;

		private Function du_dtFunc;
		public override Function du_dt => du_dtFunc;

		#endregion

		public ParsedEquation(Functions functions)
		{
			k = (x, t, u) => ParseComplexFunc(functions.K, x, t, u);
			dK_duFunc = (x, t, u) => ParseComplexFunc(functions.dK_du, x, t, u);
			gFunc = (x, t, u) => ParseG(functions.g, x, t, u, K(x, t, u));
			initCond = x => ParseCond(functions.InitCond, "x", x);
			leftBoundCond = t => ParseCond(functions.LeftBoundCond, "t", t);
			rightBoundCond = t => ParseCond(functions.RightBoundCond, "t", t);

			if (!string.IsNullOrEmpty(functions.u))
				uFunc = (x, t) => ParseFunc(functions.u, x, t);
			if (!string.IsNullOrEmpty(functions.du_dx))
				du_dxFunc = (x, t) => ParseFunc(functions.du_dx, x, t);
			if (!string.IsNullOrEmpty(functions.d2u_dx2))
				d2u_dx2Func = (x, t) => ParseFunc(functions.d2u_dx2, x, t);
			if (!string.IsNullOrEmpty(functions.du_dt))
				du_dtFunc = (x, t) => ParseFunc(functions.du_dt, x, t);
		}

		private double ParseG(string function, double x, double t, double u, double K)
		{
			parser.LocalVariables["K"] = (decimal)K;
			return ParseComplexFunc(function, x, t, u);
		}

		private double ParseComplexFunc(string function, double x, double t, double u)
		{
			parser.LocalVariables["u"] = (decimal)u;
			return ParseFunc(function, x, t);
		}

		private double ParseFunc(string function, double x, double t)
		{
			parser.LocalVariables["x"] = (decimal)x;
			parser.LocalVariables["t"] = (decimal)t;
			return Parse(function);
		}

		private double ParseCond(string function, string variable, double a)
		{
			parser.LocalVariables[variable] = (decimal)a;
			return Parse(function);
		}

		private double Parse(string function)
		{
			return (double)parser.Parse(function);
		}
	}
}
