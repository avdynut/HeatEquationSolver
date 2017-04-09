namespace HeatEquationSolver.Settings
{
	public interface IFunctions
	{
		/// <summary>
		/// f = f(x, t, u(x, t)) - kernel
		/// </summary>
		string K { get; set; }

		/// <summary>
		/// f = f(x, t, u(x, t)) - derivative of K by u
		/// </summary>
		string dK_du { get; set; }

		/// <summary>
		/// f = f(x, t, u(x, t)) - additional function
		/// </summary>
		string g { get; set; }

		/// <summary>
		/// f = f(x) = u(x, 0) - initial condition
		/// </summary>
		string InitCond { get; set; }

		/// <summary>
		/// f = f(t) = u(x1, t) - left boundary condition
		/// </summary>
		string LeftBoundCond { get; set; }

		/// <summary>
		/// f = f(t) = u(x2, t) - right boundary condition
		/// </summary>
		string RightBoundCond { get; set; }

		/// <summary>
		/// f = f(x, t) - exact solution
		/// </summary>
		string u { get; set; }

		/// <summary>
		/// f = f(x, t) - derivative of u by x
		/// </summary>
		string du_dx { get; set; }

		/// <summary>
		/// f = f(x, t) - the second partial derivative of u by x
		/// </summary>
		string d2u_dx2 { get; set; }

		/// <summary>
		/// f = f(x, t) - derivative of u by t
		/// </summary>
		string du_dt { get; set; }
	}
}
