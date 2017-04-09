namespace HeatEquationSolver.Equations
{
	public class Functions
	{
		public string K { get; set; } = "t*x*x+u*u";
		public string dK_du { get; set; } = "2*u";
		public string g { get; set; } = "x*x+6*x*t-(2*u*(2*x*t+3*t*t)*(2*x*t+3*t*t))-(2*t*K)";
		public string InitCond { get; set; } = "0";
		public string LeftBoundCond { get; set; } = "0";
		public string RightBoundCond { get; set; } = "t+3*t*t";
		public string u { get; set; } = "t*x*x+3*x*t*t";
		public string du_dx { get; set; } = "2*x*t+3*t*t";
		public string d2u_dx2 { get; set; } = "2*t";
		public string du_dt { get; set; } = "x*x+6*x*t";
	}
}
