using System.Linq;
using System.Text;

namespace HeatEquationSolver
{
    public static class Vector
    {
        public static double[] MultiplyConst(double a, double[] vector)
        {
            return vector.Select(x => a * x).ToArray();
        }

        public static string ArrayToString(double[] array)
        {
            var sb = new StringBuilder();
            foreach (double t in array)
            {
                sb.Append(t);
                sb.Append("; ");
            }
            return sb.ToString();
        }
    }
}
