﻿using System;
using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver
{
    public delegate double Function(double x, double t);
    public delegate double ComplexFunction(double x, double t, double y);

    public class Equation
    {
        public Function u;
        public ComplexFunction K;
        public ComplexFunction g;
        public ComplexFunction dK_dy;

        public Equation(Function u, ComplexFunction K, ComplexFunction g, ComplexFunction dK_dy)
        {
            this.u = u;
            this.K = K;
            this.g = g;
            this.dK_dy = dK_dy;
        }

        public void CheckEquation(Function du_dx, Function d2u_dx, Function du_dt)
        {
            Function f = (x, t) => du_dt(x, t) - dK_dy(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - K(x, t, u(x, t)) * d2u_dx(x, t) - g(x, t, u(x, t));
            if (f(4, 3) != 0)
                throw new Exception("Incorrect equation");
        }

        public double[] SubstituteValues(double t, double[] y, double[] yK)
        {
            double[] f = new double[N + 1];
            f[0] = yK[0] - u(x1, t);
            f[N] = yK[N] - u(x2, t);
            for (int n = 1; n < N; n++)
            {
                double x = n * h;
                f[n] = dK_dy(x, t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
                            K(x, t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
                            g(x, t, y[n]) - (yK[n] - y[n]) / tau;
            }
            return f;
        }
    }
}