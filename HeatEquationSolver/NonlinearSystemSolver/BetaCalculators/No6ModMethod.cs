﻿using System;

namespace HeatEquationSolver.NonlinearSystemSolver.BetaCalculators
{
    public class No6ModMethod : No6Method
    {
        public override double Multiplier => -Math.Sqrt(Beta);

        public No6ModMethod(double beta0, bool onlyUp = true) : base(beta0, onlyUp)
        {
        }

        protected override double CalculateBeta(double norm)
        {
            double nextBeta = Math.Min(1, norm0 * norm0 * gamma / (norm * norm * Beta));
            if (AssignBeta(nextBeta))
                gamma *= nextBeta / Beta;
            predNorm = norm;
            return Beta;
        }
    }
}
