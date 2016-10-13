﻿namespace HeatEquationSolver.BetaCalculators
{
    public abstract class BetaCalculator
    {
        public double Beta;
        protected double predNorm;
        protected bool onlyUp = true;
        protected bool notRecalculate;
        public abstract double Multiplier { get; }

        public virtual void Init(double beta0, double firstNorm)
        {
            Beta = beta0;
            predNorm = firstNorm;
        }

        public double NextBeta(double norm)
        {
            if (notRecalculate)
                return Beta;
            //if (norm < predNorm)
            //{
            //    notRecalculate = true;
            //    return Beta = 1;
            //}
            return CalculateBeta(norm);
        }

        protected abstract double CalculateBeta(double norm);

        protected bool AssignBeta(double nextBeta)
        {
            if (!onlyUp || (onlyUp && nextBeta > Beta))
            {
                Beta = nextBeta;
                return true;
            }
            return false;
        }
    }

    public enum MethodBeta
    {
        Puzynin,
        No6,
        ModNo6
    }
}
