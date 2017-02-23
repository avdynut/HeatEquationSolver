using System.Configuration;

namespace HeatEquationSolver
{
	public static class ResourcesHelper
	{
		public static string K => ConfigurationManager.AppSettings[nameof(K)];
		public static string dK_du => ConfigurationManager.AppSettings[nameof(dK_du)];
		public static string g => ConfigurationManager.AppSettings[nameof(g)];
		public static string InitCond => ConfigurationManager.AppSettings[nameof(InitCond)];
		public static string LeftBoundCond => ConfigurationManager.AppSettings[nameof(LeftBoundCond)];
		public static string RightBoundCond => ConfigurationManager.AppSettings[nameof(RightBoundCond)];
		public static string u => ConfigurationManager.AppSettings[nameof(u)];
		public static string du_dx => ConfigurationManager.AppSettings[nameof(du_dx)];
		public static string d2u_dx2 => ConfigurationManager.AppSettings[nameof(d2u_dx2)];
		public static string du_dt => ConfigurationManager.AppSettings[nameof(du_dt)];

		public static double X1 = double.Parse(ConfigurationManager.AppSettings[nameof(X1)]);

	}
}
