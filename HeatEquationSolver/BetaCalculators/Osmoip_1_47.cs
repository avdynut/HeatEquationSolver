using System;

namespace HeatEquationSolver.BetaCalculators
{
	/// <summary>
	/// One-step method of incomplete predict (1.47)
	/// </summary>
	public class Osmoip_1_47 : BetaCalculatorBase
	{
		public override double Multiplier => -Math.Sqrt(Beta);
		private double beta0doubleNorm0double;

		public override void Init(double beta0, double firstNorm)
		{
			beta0doubleNorm0double = beta0 * beta0 * firstNorm * firstNorm;
			base.Init(beta0, firstNorm);
		}

		protected override void CalculateBeta(double norm)
		{
			Beta = Math.Min(1, beta0doubleNorm0double / (Beta * norm * norm));
			predNorm = norm;
		}
	}
}
