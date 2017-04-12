using Differential;
using HeatEquationSolver.Settings;
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
			var k = ((Expression<Func<double, double, double, double>>)CodeDom.GetExpressionFrom($"(x, t, u) => {functions.K}")).Compile();
			K = (x, t, u) => k(x, t, u);

			var dK_duFunc = ((Expression<Func<double, double, double, double>>)CodeDom.GetExpressionFrom($"(x, t, u) => {functions.dK_du}")).Compile();
			dK_du = (x, t, u) => dK_duFunc(x, t, u);

			var gFunc = ((Expression<Func<double, double, double, double, double>>)CodeDom.GetExpressionFrom($"(x, t, u, K) => {functions.g}")).Compile();
			g = (x, t, u) => gFunc(x, t, u, K(x, t, u));

			var initCond = ((Expression<Func<double, double>>)CodeDom.GetExpressionFrom($"x => {functions.InitCond}")).Compile();
			InitCond = x => initCond(x);

			var leftBoundCond = ((Expression<Func<double, double>>)CodeDom.GetExpressionFrom($"t => {functions.LeftBoundCond}")).Compile();
			LeftBoundCond = t => leftBoundCond(t);

			var rightBoundCond = ((Expression<Func<double, double>>)CodeDom.GetExpressionFrom($"t => {functions.RightBoundCond}")).Compile();
			RightBoundCond = t => rightBoundCond(t);

			u = Compile(functions.u);
			du_dx = Compile(functions.du_dx);
			d2u_dx2 = Compile(functions.d2u_dx2);
			du_dt = Compile(functions.du_dt);
		}

		public Expression<Func<double, double, double>> Parse(string func)
		{
			return ((Expression<Func<double, double, double>>)CodeDom.GetExpressionFrom($"(x, t) => {func}")).Simplify();
		}

		private Function Compile(string func)
		{
			if (string.IsNullOrEmpty(func))
				return null;
			var f = Parse(func).Compile();
			return (x, t) => f(x, t);
		}
	}
}
