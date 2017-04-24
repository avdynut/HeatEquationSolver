﻿using System;

namespace HeatEquationSolver.BetaCalculators
{
	/// <summary>
	/// One-step method of incomplete predict (1.301)
	/// </summary>
	public class Osmoip_1_301 : BetaCalculatorBase
	{
		private double gamma;

		public override void Init(double beta0, double firstNorm)
		{
			gamma = beta0 * beta0;
			base.Init(beta0, firstNorm);
		}

		protected override void CalculateBeta(double norm)
		{
			double nextBeta = Math.Min(1, gamma * predNorm / (Beta * norm));
			gamma *= nextBeta * predNorm / (Beta * norm);
			Beta = nextBeta;
			predNorm = norm;
		}
	}
}
