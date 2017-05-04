using HeatEquationSolver.Equations;
using HeatEquationSolver.Settings;
using NUnit.Framework;
using System;
using System.Linq;

namespace HeatEquationSolver.Tests
{
	public class ModelEquationTests
	{
		private ISettings settings;
		private Solver solver;
		private HeatEquation equation;

		[OneTimeSetUp]
		public void Init()
		{
			settings = DataManager.Settings;
			settings.UseParsedEquation = false;
			equation = new ModelEquation();
			solver = new Solver(settings);
			solver.Solve();
		}

		[Test]
		public void CheckModelEquation()
		{
			var r = new Random();
			Assert.That(equation.SubstituteValues(r.NextDouble(), r.NextDouble()).Round(12), Is.EqualTo(0), "Incorrect equation");
		}

		[Test]
		public void SolverShouldFindCorrectApporximatedValues()
		{
			double[] expectedValues = { 0, 0.3101042406, 0.6400414248, 0.9900327861, 1.3600262915, 1.7500207651, 2.1600158569, 2.5900114047, 3.0400073153, 3.5100035281, 4 };
			var approximatedValues = solver.Answer.Select(x => Math.Round(x, 10));
			Assert.That(approximatedValues, Is.EqualTo(expectedValues), "Incorrect answer");
		}

		[Test]
		public void ApproximatedValuesShouldResembleToExactValues()
		{
			var exactValues = new double[settings.N + 1];
			var h = (settings.X2 - settings.X1) / settings.N;
			for (int i = 0; i <= settings.N; i++)
				exactValues[i] = Math.Round(equation.u(settings.X1 + i * h, settings.T2), 3);
			var approximatedValues = solver.Answer.Select(x => Math.Round(x, 3));
			Assert.That(approximatedValues, Is.EqualTo(exactValues), "Incorrect answer");
		}

		[Test]
		public void SolverShouldReturnCorrectNorm()
		{
			double expectedNorm = 3.9E-05;
			Assert.That(Math.Round(solver.Norm, 6), Is.EqualTo(expectedNorm), "Incorrect norm");
		}
	}
}
