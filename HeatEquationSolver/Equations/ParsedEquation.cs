using HeatEquationSolver.Settings;
using MathExpressionsNet;
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
			var k = Parse<Func<double, double, double, double>>(functions.K, "x", "t", "u").Compile();
			K = (x, t, u) => k(x, t, u);

			var dK_duFunc = Parse<Func<double, double, double, double>>(functions.dK_du, "x", "t", "u").Compile();
			dK_du = (x, t, u) => dK_duFunc(x, t, u);

			var gFunc = Parse<Func<double, double, double, double, double>>(functions.g, "x", "t", "u", "K").Compile();
			g = (x, t, u) => gFunc(x, t, u, K(x, t, u));

			var initCond = Parse<Func<double, double>>(functions.InitCond, "x").Compile();
			InitCond = x => initCond(x);

			var leftBoundCond = Parse<Func<double, double>>(functions.LeftBoundCond, "t").Compile();
			LeftBoundCond = t => leftBoundCond(t);

			var rightBoundCond = Parse<Func<double, double>>(functions.RightBoundCond, "t").Compile();
			RightBoundCond = t => rightBoundCond(t);

			u = Compile(functions.u);
			du_dx = Compile(functions.du_dx);
			d2u_dx2 = Compile(functions.d2u_dx2);
			du_dt = Compile(functions.du_dt);
		}

		public Expression<T> Parse<T>(string func, params string[] variables)
		{
			return CodeDom.ParseExpression<T>(func, variables);
		}

		private Function Compile(string func)
		{
			if (string.IsNullOrEmpty(func))
				return null;
			var f = Parse<Func<double, double, double>>(func, "x", "t").Compile();
			return (x, t) => f(x, t);
		}

		public Expression<T> Derivative<T>(Expression<T> expr, string byVariable)
		{
			return expr.Derive(byVariable).Simplify();
		}
	}
}
