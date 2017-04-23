using System;

namespace HeatEquationSolver.BetaCalculators
{
	/// <summary>
	/// One-step method of incomplete predict
	/// </summary>
	public class Osmoip : Osmoip_1_274
	{
		public override double Multiplier => -Math.Sqrt(Beta);

		protected override void CalculateBeta(double norm)
		{
			double nextBeta = Math.Min(1, norm0 * norm0 * gamma / (norm * norm * Beta));
			gamma *= nextBeta / Beta;
			Beta = nextBeta;
			predNorm = norm;
		}
	}
}
