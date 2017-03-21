using HeatEquationSolver;
using System;
using System.Threading;

namespace ConsoleSolver
{
	class Program
	{
		static void Main(string[] args)
		{
			var dm = new DataManager();
			dm.SaveToJson();
			//var solver = new Solver(new Settings { M = 100 });
			//solver.Solve(new CancellationToken());

			//double t = 0;
			//for (int i = 0; i < 20; i++)
			//	Run(t += 1);
			Console.Read();
		}

		private static void Run(double t)
		{
			var qn = new Solver(new Settings { T2 = t });
			qn.Solve(new CancellationToken());
			Console.WriteLine($"{t}\t{qn.Norm}");
		}
	}
}
