using System;

namespace HeatEquationSolver.Equations
{
    public delegate double InitialCondition(double x);
    public delegate double BoundaryCondtion(double t);
    public delegate double Function(double x, double t);
    public delegate double ComplexFunction(double x, double t, double u);

    public abstract class HeatEquation
    {
        public abstract ComplexFunction K { get; }
        public abstract ComplexFunction dK_du { get; }
        public abstract ComplexFunction g { get; }
        public abstract InitialCondition InitCond { get; }
        public abstract BoundaryCondtion LeftBoundCond { get; }
        public abstract BoundaryCondtion RightBoundCond { get; }

        #region Optional
        public abstract Function u { get; }
        public abstract Function du_dx { get; }
        public abstract Function d2u_dx2 { get; }
        public abstract Function du_dt { get; }
        #endregion

        public bool IsEquationCorrect()
        {
            Function f = (x, t) => du_dt(x, t) - dK_du(x, t, u(x, t)) * Math.Pow(du_dx(x, t), 2) - K(x, t, u(x, t)) * d2u_dx2(x, t) - g(x, t, u(x, t));
            var r = new Random();
            return f(r.Next(), r.Next()) == 0;
        }
    }
}
