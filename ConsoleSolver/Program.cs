using HeatEquationSolver;
using HeatEquationSolver.Settings;
using HeatEquationSolverUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleSolver
{
	class Program
	{
		private static ISettings settings = new Settings { UseParsedEquation = false, Epsilon2 = 1e-4 };
		private static StreamWriter writer;
		private const int Count = 3;

		static void Main(string[] args)
		{
			handler = new ConsoleEventDelegate(ConsoleEventCallback);
			SetConsoleCtrlHandler(handler, true);

			writer = new StreamWriter("different.txt");
			var methods = MethodBeta.Methods;//.Where((m, i) => i == 0 || i == 2);
			var ns = new[] { 100 };
			//var epsilons = new[] { 1e-4, 1e-6, 1e-8, 1e-10 };
			//var epsilons2 = new[] { 1e-6 };
			var betas = new[] { 0.1, 0.01 };
			writer.WriteLine($"Params = {string.Join("\t", betas)}");
			Thread.Sleep(3000);

			foreach (var n in ns)
			{
				settings.N = n;
				writer.WriteLine($"N = {n}");
				Console.WriteLine($"N = {n}");

				foreach (var par in betas)
				{
					//settings.Epsilon = par;
					//settings.Epsilon2 = par;
					settings.Beta0 = par;
					writer.Write($"{par}   \t");
					Console.WriteLine(par);

					foreach (var method in methods)
					{
						settings.BetaCalculatorMethod = method.BetaCalculator;
						double seconds = GetAverageSeconds();
						writer.Write(seconds + "\t");
					}
					writer.WriteLine();
				}
				writer.WriteLine();
				Console.WriteLine();
			}
			writer.Close();
		}

		private static double GetAverageSeconds()
		{
			double sum = 0;
			for (int i = 0; i < Count; i++)
			{
				settings.M = 100;
				var solver = new Solver(settings);
				var sw = new Stopwatch();
				sw.Start();
				solver.Solve();
				sw.Stop();
				sum += sw.Elapsed.TotalSeconds;
			}
			return Math.Round(sum / Count, 3);
		}

		private static double GetAverageIterations()
		{
			settings.M = 100;
			var solver = new Solver(settings);
			solver.Solve();
			return Math.Round(solver.AverageIterationsOnLayer, 1);
		}

		static bool ConsoleEventCallback(int eventType)
		{
			writer.Close();
			return true;
		}

		static ConsoleEventDelegate handler;
		private delegate bool ConsoleEventDelegate(int eventType);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
	}
}
