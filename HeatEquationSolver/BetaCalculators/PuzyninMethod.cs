using System;

namespace HeatEquationSolver.BetaCalculators
{
	/// <summary>
	/// One-step method of incomplete predict of I.V. Puzynin
	/// </summary>
	public class PuzyninMethod : BetaCalculatorBase
	{
		protected override void CalculateBeta(double norm)
		{
			Beta = Math.Min(1, Beta * predNorm / norm);
			predNorm = norm;
		}
	}
}
