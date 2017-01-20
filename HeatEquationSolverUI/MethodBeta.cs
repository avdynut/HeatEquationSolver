using HeatEquationSolver.BetaCalculators;

namespace HeatEquationSolverUI
{
    public class MethodBeta
    {
        public BetaCalculatorBase BetaCalculator { get; }
        public string Name { get; }
        public string RelativePathToImage { get; }

        public MethodBeta(BetaCalculatorBase betaCalculator, string name, string relativePathToImage)
        {
            BetaCalculator = betaCalculator;
            Name = name;
            RelativePathToImage = relativePathToImage;
        }
    }
}
