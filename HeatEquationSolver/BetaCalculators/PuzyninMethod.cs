using System;

namespace HeatEquationSolver.BetaCalculators
{
    public class PuzyninMethod : BetaCalculator
    {
        public override double Multiplier => -Beta;

        public PuzyninMethod(double beta0, bool onlyUp = true) : base(beta0, onlyUp)
        {
        }

        protected override double CalculateBeta(double norm)
        {
            double nextBeta = Math.Min(1, Beta * predNorm / norm);
            AssignBeta(nextBeta);
            predNorm = norm;
            return Beta;
        }
    }
}
