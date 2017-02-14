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
        public ComplexFunction dK_du;
        public InitialCondition InitCond;
        public BoundaryCondtion LeftBoundCond;
        public BoundaryCondtion RightBoundCond;

        public HeatEquation(ComplexFunction K, ComplexFunction dK_du, ComplexFunction g, InitialCondition initCond, BoundaryCondtion leftBoundCond, BoundaryCondtion rightBoundCond)
        {
            this.K = K;
            this.g = g;
            this.dK_du = dK_du;
            InitCond = initCond;
            LeftBoundCond = leftBoundCond;
            RightBoundCond = rightBoundCond;
        }

        public bool IsEquationCorrect(Function du_dx, Function d2u_dx2, Function du_dt)
        {
            Function f = (x, t) => du_dt(x, t) - dK_du(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - K(x, t, u(x, t)) * d2u_dx2(x, t) - g(x, t, u(x, t));
            var r = new Random();
            return f(r.Next(), r.Next()) == 0;
        }
    }
}
