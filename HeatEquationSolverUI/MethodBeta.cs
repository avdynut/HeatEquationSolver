using HeatEquationSolver.BetaCalculators;
using System;

namespace HeatEquationSolverUI
{
	public class MethodBeta
	{
		public BetaCalculator BetaCalculator { get; }
		public string Name { get; }
		public string PathToImage { get; }

		public MethodBeta(BetaCalculator betaCalculator, string name, string pictureFileName)
		{
			BetaCalculator = betaCalculator;
			Name = name;
			PathToImage = $@"{Environment.CurrentDirectory}\Images\MethodsForBeta\{pictureFileName}";
		}

		public static readonly MethodBeta[] Methods = {
			new MethodBeta(BetaCalculator.Puzynin, "I. Метод И.В. Пузынина", "puzynin.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_5, "II. Одношаговый метод неполного прогноза (1.5)", "osmoip_1_5.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_47, "III. Одношаговый метод неполного прогноза (1.47)", "osmoip_1_47.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_267, "IV. Одношаговый метод неполного прогноза (1.267)", "osmoip_1_267.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_301, "V. Одношаговый метод неполного прогноза (1.301)", "osmoip_1_301.PNG"),
			new MethodBeta(BetaCalculator.Osmoip_1_274, "VI. Одношаговый метод неполного прогноза (1.274)", "osmoip_1_274.PNG") };
	}
}
