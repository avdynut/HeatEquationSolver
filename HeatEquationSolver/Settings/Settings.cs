﻿using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Equations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace HeatEquationSolver.Settings
{
	public class Settings : ISettings
	{
		public double X1 { get; set; } = 0;
		public double X2 { get; set; } = 1;

		public double T1 { get; set; } = 0;
		public double T2 { get; set; } = 1;

		public int N { get; set; } = 10;
		public int M { get; set; } = 10;

		public double Epsilon { get; set; } = 1e-5;
		public double Epsilon2 { get; set; } = 1e-4;

		public double Alpha { get; set; } = 1e-10;
		public double Beta0 { get; set; } = 0.01;

		[JsonConverter(typeof(StringEnumConverter))]
		public BetaCalculator BetaCalculatorMethod { get; set; } = BetaCalculators.BetaCalculator.Puzynin;

		public int MaxIterations { get; set; } = 50000;

		public bool UseParsedEquation { get; set; } = true;
		public IFunctions Functions { get; set; } = new Functions();

		[JsonIgnore]
		public BetaCalculatorBase BetaCalculator
		{
			get
			{
				switch (BetaCalculatorMethod)
				{
					case BetaCalculators.BetaCalculator.Puzynin:
						return new PuzyninMethod();
					case BetaCalculators.BetaCalculator.Osmoip_1_5:
						return new Osmoip_1_5();
					case BetaCalculators.BetaCalculator.Osmoip_1_47:
						return new Osmoip_1_47();
					case BetaCalculators.BetaCalculator.Osmoip_1_267:
						return new Osmoip_1_267();
					case BetaCalculators.BetaCalculator.Osmoip_1_274:
						return new Osmoip_1_274();
					case BetaCalculators.BetaCalculator.Osmoip_1_301:
						return new Osmoip_1_301();
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		[JsonIgnore]
		public HeatEquation Equation => UseParsedEquation ? new ParsedEquation(Functions) : (HeatEquation)new ModelEquation();
	}
}
