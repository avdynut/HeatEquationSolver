using HeatEquationSolver.BetaCalculators;

namespace HeatEquationSolverUI
{
	public class MethodBeta
	{
		public BetaCalculator BetaCalculator { get; }
		public string Name { get; }
		public string RelativePathToImage { get; }

		public MethodBeta(BetaCalculator betaCalculator, string name, string relativePathToImage)
		{
			BetaCalculator = betaCalculator;
			Name = name;
			RelativePathToImage = relativePathToImage;
		}
	}
}
