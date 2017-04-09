using HeatEquationSolver.BetaCalculators;

namespace HeatEquationSolverUI
{
	public class MethodBeta
	{
		public BetaCalculator BetaCalculator { get; }
		public string Name { get; }
		public string RelativePathToImage { get; }

		public MethodBeta(BetaCalculator betaCalculator, string name, string pictureFileName)
		{
			BetaCalculator = betaCalculator;
			Name = name;
			RelativePathToImage = $@"Images\MethodsForBeta\{pictureFileName}";
		}

		public static readonly MethodBeta[] Methods = {
			new MethodBeta(BetaCalculator.Puzynin, "Метод Пузынина", "puzynin.PNG"),
			new MethodBeta(BetaCalculator.No6, "Нерегуляризованный одношаговый метод", "no6.PNG"),
			new MethodBeta(BetaCalculator.No6Mod, "Модифицированный НО метод", "no6_mod.PNG") };
	}
}
