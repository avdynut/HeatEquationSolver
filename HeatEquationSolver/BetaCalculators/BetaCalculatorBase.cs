namespace HeatEquationSolver.BetaCalculators
{
	public abstract class BetaCalculatorBase
	{
		protected double predNorm;
		public virtual double Multiplier => -Beta;
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
		Osmoip_1_5,
		Osmoip_1_47,
		Osmoip_1_267,
		Osmoip_1_274,
		Osmoip_1_301
	}
}
