namespace HeatEquationSolver.Equations
{
	public delegate double InitialCondition(double x);
	public delegate double BoundaryCondtion(double t);
	public delegate double Function(double x, double t);
	public delegate double ComplexFunction(double x, double t, double u);

	public abstract class HeatEquation
	{
		public virtual ComplexFunction K { get; protected set; }
		public virtual ComplexFunction dK_du { get; protected set; }
		public virtual ComplexFunction g { get; protected set; }
		public virtual InitialCondition InitCond { get; protected set; }
		public virtual BoundaryCondtion LeftBoundCond { get; protected set; }
		public virtual BoundaryCondtion RightBoundCond { get; protected set; }

		#region Optional
		public virtual Function u { get; protected set; }
		public virtual Function du_dx { get; protected set; }
		public virtual Function d2u_dx2 { get; protected set; }
		public virtual Function du_dt { get; protected set; }
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
