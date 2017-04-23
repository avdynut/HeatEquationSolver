namespace HeatEquationSolver.BetaCalculators
{
	public abstract class BetaCalculatorBase
	{
		protected double predNorm;
		public abstract double Multiplier { get; }
		public double Beta { get; protected set; }

		public virtual void Init(double beta0, double firstNorm)
		{
			Beta = beta0;
			predNorm = firstNorm;
		}

		public void CalculateNextBeta(double norm)
		{
			if (norm < predNorm)
				Beta = 1;
			else
				CalculateBeta(norm);
		}

		protected abstract void CalculateBeta(double norm);
	}

	public enum BetaCalculator
	{
		Puzynin,
		Osmoip_1_274,
		Osmoip
	}
}
