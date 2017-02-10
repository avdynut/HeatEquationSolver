using System;

namespace HeatEquationSolver
{
	public static class ModelEquation
	{
		public static double K(double x, double t, double u)
		{
			return x * x * t + u * u;
		}

		public static double g(double x, double t, double u)
		{
			return x * x + 6 * x * t - 2 * u * Math.Pow(2 * x * t + 3 * t * t, 2) - 2 * t * (x * x * t + u * u);
		}

		public static double InitCond(double x)
		{
			return 0;
		}

		public static double LeftBoundCond(double t)
		{
			return 0;
		}

		public static double RightBoundCond(double t)
		{
			return t + 3 * t * t;
		}

		#region Optional

		public static double u(double x, double t)
		{
			return x * x * t + 3 * x * t * t;
		}

		public static double dK_du(double x, double t, double u)
		{
			return 2 * u;
		}

		public static double du_dx(double x, double t)
		{
			return 2 * x * t + 3 * t * t;
		}

		public static double d2u_dx2(double x, double t)
		{
			return 2 * t;
		}

		public static double du_dt(double x, double t)
		{
			return x * x + 6 * x * t;
		}

		#endregion

	}
}
