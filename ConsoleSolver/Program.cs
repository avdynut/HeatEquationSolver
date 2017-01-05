using HeatEquationSolver;
using System;
using System.Threading;

namespace ConsoleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var eq = new Equation(u, k, g, (x, t) => 2 * x * t, (x, t) => 2 * t, (x, t) => x * x, (x, t, u) => 2 * u);

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

        private static double U(double x, double t)
        {
            return x * x * t;
        }

        private static double K(double x, double t, double u)
        {
            return u * u;
        }

        private static double G(double x, double t, double u)
        {
            return x * x - 8 * x * x * t * t * u - 2 * t * u * u;
        }
    }
}
