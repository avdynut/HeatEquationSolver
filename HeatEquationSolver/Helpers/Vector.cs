using System.Linq;

namespace HeatEquationSolver.Helpers
{
	public static class Vector
	{
		public static double[] MultiplyConst(this double[] vector, double a)
		{
			return vector.Select(x => a * x).ToArray();
		}

		public static string AsString(this double[] array)
		{
			return string.Join("; ", array);
		}
	}
}
