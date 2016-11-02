using System.Linq;

namespace HeatEquationSolver
{
    public static class Vector
    {
        public static double[] MultiplyConst(double a, double[] vector)
        {
            return vector.Select(x => a * x).ToArray();
        }
    }
}
