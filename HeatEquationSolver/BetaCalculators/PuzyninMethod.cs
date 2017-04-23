using System;

namespace HeatEquationSolver.BetaCalculators
{
	public class PuzyninMethod : BetaCalculatorBase
	{
		public override double Multiplier => -Beta;

		protected override void CalculateBeta(double norm)
		{
			Beta = Math.Min(1, Beta * predNorm / norm);
			predNorm = norm;
		}
	}
}
