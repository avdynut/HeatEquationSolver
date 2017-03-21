namespace HeatEquationSolver.Equations
{
	public class Functions
	{
		public string K { get; set; } = "t*x^2+u^2";
		public string dK_du { get; set; } = "2u";
		public string g { get; set; } = "x^2+6x*t-2u*(2x*t+3t^2)^2-2t*K";
		public string InitCond { get; set; } = "0";
		public string LeftBoundCond { get; set; } = "0";
		public string RightBoundCond { get; set; } = "t+3t^2";
		public string u { get; set; } = "t*x^2+3x*t^2";
		public string du_dx { get; set; } = "2x*t+3t^2";
		public string d2u_dx2 { get; set; } = "2t";
		public string du_dt { get; set; } = "x^2+6x*t";
	}
}
