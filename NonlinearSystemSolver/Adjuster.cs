using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NonlinearSystemSolver.BetaCalculators;

namespace NonlinearSystemSolver
{
    public class Adjuster
    {
        public void SetUp(int n, double epsilon, double beta0, MethodBeta methodBeta)
        {
            BetaCalculator betaCalculator = null;
            switch (methodBeta)
            {
                case MethodBeta.Puzynin:
                    betaCalculator = new PuzyninMethod(beta0, true);
                    break;
                case MethodBeta.No6:
                    betaCalculator = new No6Method(beta0);
                    break;
                case MethodBeta.ModNo6:
                    betaCalculator = new No6ModMethod(beta0);
                    break;
                default:
                    new ArgumentException("Incorrect value of method for calculating beta");
                    break;
            }
            var solver = new Solver(n, epsilon, betaCalculator);
        }
    }
}
