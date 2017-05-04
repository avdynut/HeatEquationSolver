using HeatEquationSolver;
using HeatEquationSolver.Settings;
using HeatEquationSolverUI;
using System;
using System.Diagnostics;

namespace ConsoleSolver
{
	class Program
	{
		private static ISettings settings = new Settings { M = 100, UseParsedEquation = false, N = 10 };

		static void Main(string[] args)
		{
			foreach (var method in MethodBeta.Methods)
			{
				Console.WriteLine(method.Name);
				settings.BetaCalculatorMethod = method.BetaCalculator;
				SolveWithDifferentBeta(0.001, 0.1, 11);
				Console.WriteLine();
			}
			Console.Read();
		}

		private static void SolveWithDifferentBeta(double firstBeta, double step, int count)
		{
			settings.Beta0 = firstBeta;
			for (int i = 1; i <= count; i++)
			{
				var solver = new Solver(settings);
				var sw = new Stopwatch();
				sw.Start();
				solver.Solve();
				sw.Stop();
				Console.WriteLine($"{settings.Beta0}\t{sw.Elapsed.TotalSeconds}\t{solver.Norm}");
				settings.Beta0 = firstBeta / step * i;
			}
		}
	}
}
