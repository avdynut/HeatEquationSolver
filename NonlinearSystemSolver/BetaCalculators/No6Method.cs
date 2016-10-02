﻿using System;

namespace NonlinearSystemSolver.BetaCalculators
{
    public class No6Method : BetaCalculator
    {
        public override double Multiplier => -beta;

        protected double norm0;
        protected double gamma;

        public No6Method(double beta0, bool onlyUp = true) : base(beta0, onlyUp)
        {
            gamma = beta0 * beta0;
        }

        public override void Init(double firstNorm)
        {
            norm0 = firstNorm;
            base.Init(firstNorm);
        }

        protected override double CalculateBeta(double norm)
        {
            double nextBeta = Math.Min(1, norm0 * gamma / (norm * beta));
            if (AssignBeta(nextBeta))
                gamma *= nextBeta / beta;
            predNorm = norm;
            return beta;
        }
    }
}
