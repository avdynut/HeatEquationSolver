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

		public double SubstituteValues(double x, double t)
		{
			return GetFunctionValue(du_dt(x, t), dK_du(x, t, u(x, t)), du_dx(x, t), K(x, t, u(x, t)), d2u_dx2(x, t), g(x, t, u(x, t)));
		}

		public double SubstituteValues(double x, double t, double u, double du_dt, double du_dx, double d2u_dx2)
		{
			return GetFunctionValue(du_dt, dK_du(x, t, u), du_dx, K(x, t, u), d2u_dx2, g(x, t, u));
		}

		private double GetFunctionValue(double du_dt, double dK_du, double du_dx, double K, double d2u_dx2, double g)
		{
			return du_dt - dK_du * du_dx * du_dx - K * d2u_dx2 - g;
		}
	}
}
