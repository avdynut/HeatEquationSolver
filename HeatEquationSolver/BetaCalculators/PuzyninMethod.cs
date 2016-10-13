using System;

namespace HeatEquationSolver.BetaCalculators
{
    public class PuzyninMethod : BetaCalculator
    {
        public override double Multiplier => -Beta;

        protected override double CalculateBeta(double norm)
        {
            double nextBeta = Math.Min(1, Beta * predNorm / norm);
            AssignBeta(nextBeta);
            predNorm = norm;
            return Beta;
        }
    }
}
