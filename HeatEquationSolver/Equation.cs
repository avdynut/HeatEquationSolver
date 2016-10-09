using System;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver
{
    public delegate double Function(double x, double t);
    public delegate double ComplexFunction(double x, double t, double y);

    public class Equation
    {
        public Function u;
        public Function f;
        public ComplexFunction K;
        private ComplexFunction g;
        public ComplexFunction dK_dy;

        public Equation(Function u, ComplexFunction k, ComplexFunction g, ComplexFunction dk_du,
            Function du_dx = null, Function d2u_dx = null, Function du_dt = null)
        {
            this.u = u;
            f = (x, t) => du_dt(x, t) - dk_du(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - k(x, t, u(x, t)) * d2u_dx(x, t) - g(x, t, u(x, t));
            this.K = k;
            this.g = g;
            this.dK_dy = dk_du;
        }

        public double[] SubstituteValues(double t, double[] y, double[] yK)
        {
            double[] f = new double[N + 1];
            double x;
            f[0] = yK[0] - u(x1, t);
            f[N] = yK[N] - u(x2, t);
            for (int n = 1; n < N; n++)
            {
                x = n * h;
                f[n] = dK_dy(x, t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                            K(x, t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                            g(x, t, y[n]) - (yK[n] - y[n]) / tau;
                //f[n] = (yK[n] - y[n]) / tau
                //     - KDy(x, t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2)
                //     - K(x, t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h)
                //     - g(x, t, y[n]);
            }
            return f;
        }
    }
}
