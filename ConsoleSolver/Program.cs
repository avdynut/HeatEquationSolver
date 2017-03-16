using HeatEquationSolver;
using HeatEquationSolver.Equations;
using org.mariuszgromada.math.mxparser;
using SimpleExpressionEvaluator;
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


            //Settings.M = 100;
            //double t = 0;
            //for (int i = 0; i < 20; i++)
            //    Run(t += 1);

            string u_ = "x*x*t+3*x*t*t";
            string K_ = "x*x*t+u*u";
            string g_ = "x*x+6*x*t-2*u*(2*x*t+3*t*t)*(2*x*t+3*t*t)-2*t*K";
            var x = 3;
            var t = 5;

            var en = new ExpressionEvaluator();
            var uf = en.Compile(u_);
            var Kf = en.Compile(K_);
            var gf = en.Compile(g_);
            var u = uf(new { x, t });
            var K = Kf(new { x, t, u });
            var g = gf(new { x, t, u, K });

            // var result = g(x, t, u(x, t), K(x, t, u(x, t)));
            var eq = new ModelEquation();
            var expected = eq.g(x, t, eq.u(x, t));

            //HeatEquationSolver.Equations.Function uFunc = (x, t) =>
            //{
            //    var xx = new Argument("x", x);
            //    var tt = new Argument("t", t);
            //    var e = new Expression(u_, xx, tt);
            //    return e.calculate();
            //};
            //var d = uFunc(3, 5);
            //var processor = new Processor();
            //var d = processor.Parse(u_);
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
