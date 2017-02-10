using System;

namespace HeatEquationSolver
{
    public delegate double InitialCondition(double x);
    public delegate double BoundaryCondtion(double t);
    public delegate double Function(double x, double t);
    public delegate double ComplexFunction(double x, double t, double u);

    public class HeatEquation
    {
        public Function u;
        public ComplexFunction K;
        public ComplexFunction g;
        public ComplexFunction dK_dy;
        public InitialCondition InitCond;
        public BoundaryCondtion LeftBoundCond;
        public BoundaryCondtion RightBoundCond;

        public HeatEquation(ComplexFunction K, ComplexFunction g, ComplexFunction dK_dy, InitialCondition initCond, BoundaryCondtion leftBoundCond, BoundaryCondtion rightBoundCond)
        {
            this.K = K;
            this.g = g;
            this.dK_dy = dK_dy;
            InitCond = initCond;
            LeftBoundCond = leftBoundCond;
            RightBoundCond = rightBoundCond;
        }

        public void CheckEquation(Function du_dx, Function d2u_dx, Function du_dt)
        {
            Function f = (x, t) => du_dt(x, t) - dK_dy(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - K(x, t, u(x, t)) * d2u_dx(x, t) - g(x, t, u(x, t));
            if (f(4, 3) != 0)
                throw new Exception("Incorrect equation");
        }
    }
}
