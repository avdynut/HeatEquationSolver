using System;

namespace HeatEquationSolver.BetaCalculators
{
	/// <summary>
	/// One-step method of incomplete predict (1.267)
	/// </summary>
	public class Osmoip_1_267 : BetaCalculatorBase
	{
		public override double Multiplier => -Math.Sqrt(Beta);

		protected override void CalculateBeta(double norm)
		{
			Beta = Math.Min(1, Beta * predNorm * predNorm / (norm * norm));
			predNorm = norm;
		}
	}
}
