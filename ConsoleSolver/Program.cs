using HeatEquationSolver;
using HeatEquationSolver.Equations;
using System;
using System.Threading;
using Z.Expressions;

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

            //string u_ = "x*x*t+3*x*t*t";
            //string K_ = "x*x*t+u*u";
            //string g_ = "x*x+6*x*t-2*u*Math.Pow(2*x*t+3*t*t,2)-2*t*K";
            ////string g_ = "x*x+6*x*t-2*u*(2*x*t+3*t*t)*(2*x*t+3*t*t)-2*t*K";
            //var x = 3;
            //var t = 5;
            //Function u = (a, b) => Eval.Compile<Func<double, double, double>>(u_, "x", "t")(a, b);
            //ComplexFunction K = (a, b, c) => Eval.Compile<Func<double, double, double, double>>(K_, "x", "t", "u")(a, b, c);
            //var g = Eval.Compile<Func<double, double, double, double, double>>(g_, "x", "t", "u", "K");
            //var result = g(x, t, u(x, t), K(x, t, u(x, t)));
            //var eq = new ModelEquation();
            //var expected = eq.g(x, t, eq.u(x, t));
            //var en = new ExpressionEvaluator();
            //var uf = en.Compile(u_);
            //var Kf = en.Compile(K_);
            //var gf = en.Compile(g_);
            //var u = uf(new { x, t });
            //var K = Kf(new { x, t, u });
            //var g = gf(new { x, t, u, K });
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
