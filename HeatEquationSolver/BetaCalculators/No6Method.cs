using System;

namespace HeatEquationSolver.BetaCalculators
{
    public class No6Method : BetaCalculator
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

        protected override double CalculateBeta(double norm)
        {
            double nextBeta = Math.Min(1, norm0 * gamma / (norm * Beta));
            if (AssignBeta(nextBeta))
                gamma *= nextBeta / Beta;
            predNorm = norm;
            return Beta;
        }
    }
}
