using System;

namespace HeatEquationSolver
{
    public delegate double Function(double x, double t);
    public delegate double ComplexFunction(double x, double t, double u);

    public class Equation
    {
        public Function f;

        public Equation(Function u, ComplexFunction k, ComplexFunction g,
            Function du_dx, Function d2u_dx, Function du_dt, ComplexFunction dk_du)
        {
            f = (x, t) => du_dt(x, t) - dk_du(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - k(x, t, u(x, t)) * d2u_dx(x, t) - g(x, t, u(x, t));
        }
    }
}
