using System;

namespace NonlinearSystemSolver.BetaCalculators
{
    public class PuzyninMethod : BetaCalculator
    {
        public override double Multiplier => -beta;

        public PuzyninMethod(double beta0, bool onlyUp = true) : base(beta0, onlyUp)
        {
        }

        protected override double CalculateBeta(double norm)
        {
            double nextBeta = Math.Min(1, beta * predNorm / norm);
            AssignBeta(nextBeta);
            predNorm = norm;
            return beta;
        }
    }
}
