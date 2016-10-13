using System;
using System.Linq;

namespace HeatEquationSolver
{
    public static class Vector
    {
        public static double[] MultiplyConst(double a, double[] vector)
        {
            return vector.Select(x => a * x).ToArray();
        }

        //public static double[] Add(double[] v, double[] u)
        //{
        //    if (v.Length != u.Length)
        //        throw new ArgumentException("Lengths of vectors must match");

        //    return v.Zip(u, (x, y) => x + y).ToArray();
        //}

        public static double[] GenerateRandom(double a, double b, int count)
        {
            if (a > b)
                throw new ArgumentException("The left end must be less then right end of segment.");

            Random r = new Random();
            double[] x0 = new double[count];
            double length = b - a;

            for (int i = 0; i < count; i++)         // заполняем рандомно вектор первого приближения
                x0[i] = a + length * r.NextDouble();

            return x0;
        }
    }
}
