using HeatEquationSolver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSolver
{
    class Program
    {
        private static CancellationTokenSource source = new CancellationTokenSource();

        static void Main(string[] args)
        {
            //var eq = new Equation(u, k, g, (x, t) => 2 * x * t, (x, t) => 2 * t, (x, t) => x * x, (x, t, u) => 2 * u);

            Settings.M = 100;
            double t = 0;
            for (int i = 0; i < 20; i++)
                Run(t += 1);
            Console.Read();
        }

        private async static void Run(double t)
        {
            Settings.T2 = t;
            var qn = new Solver();
            qn.Solve(source.Token);
            Console.WriteLine($"{t}\t{qn.Norm}");
        }

        private static double u(double x, double t)
        {
            return x * x * t;
        }

        private static double k(double x, double t, double u)
        {
            return u * u;
        }

        private static double g(double x, double t, double u)
        {
            return x * x - 8 * x * x * t * t * u - 2 * t * u * u;
        }
    }
}
