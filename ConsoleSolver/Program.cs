using HeatEquationSolver;
using System;
using System.Threading;

namespace ConsoleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var solver = new Solver();
            solver.Solve(new CancellationToken());

            Settings.M = 100;
            double t = 0;
            for (int i = 0; i < 20; i++)
                Run(t += 1);
            Console.Read();
        }

        private static void Run(double t)
        {
            Settings.T2 = t;
            var qn = new Solver();
            qn.Solve(new CancellationToken());
            Console.WriteLine($"{t}\t{qn.Norm}");
        }
    }
}
