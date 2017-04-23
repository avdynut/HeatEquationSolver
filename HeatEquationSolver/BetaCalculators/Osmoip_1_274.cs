using System;

namespace HeatEquationSolver.BetaCalculators
{
	/// <summary>
	/// One-step method of incomplete predict (1.274)
	/// </summary>
	public class Osmoip_1_274 : BetaCalculatorBase
	{
		public override double Multiplier => -Beta;

		protected double norm0;
		protected double gamma;

		public override void Init(double beta0, double firstNorm)
		{
			gamma = beta0 * beta0;
			norm0 = firstNorm;
			base.Init(beta0, firstNorm);
		}

		protected override void CalculateBeta(double norm)
		{
			double nextBeta = Math.Min(1, norm0 * gamma / (norm * Beta));
			gamma *= nextBeta / Beta;
			Beta = nextBeta;
			predNorm = norm;
		}
	}
}
