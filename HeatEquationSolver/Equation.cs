using System;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver
{
    public delegate double Function(double x, double t);
    public delegate double ComplexFunction(double x, double t, double u);

    public class Equation
    {
        public Function u;
        public Function f;

        public Equation(Function u, ComplexFunction k, ComplexFunction g,
            Function du_dx, Function d2u_dx, Function du_dt, ComplexFunction dk_du)
        {
            this.u = u;
            f = (x, t) => du_dt(x, t) - dk_du(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - k(x, t, u(x, t)) * d2u_dx(x, t) - g(x, t, u(x, t));
        }

        public double[] SubstituteValues(double[] x)
        {
            var answer = new double[N];
            //answer[0] = x[0] - u(x1, t);
            return answer;
        }
    }
}
