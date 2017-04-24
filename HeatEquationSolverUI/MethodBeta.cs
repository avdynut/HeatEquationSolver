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
			new MethodBeta(BetaCalculator.Puzynin, "Метод И.В. Пузынина", "puzynin.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_5, "Одношаг. метод неполн. прогноза (1.5)", ""),
			new MethodBeta(BetaCalculator.Osmoip_1_47, "Одношаг. метод неполн. прогноза (1.47)", ""),
			new MethodBeta(BetaCalculator.Osmoip_1_267, "Одношаг. метод неполн. прогноза (1.267)", ""),
			new MethodBeta(BetaCalculator.Osmoip_1_274, "Одношаг. метод неполн. прогноза (1.274)", "osmoip_1_274.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_301, "Одношаг. метод неполн. прогноза (1.301)", "") };
	}
}
