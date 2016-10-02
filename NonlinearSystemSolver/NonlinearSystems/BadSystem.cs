using System;

namespace NonlinearSystemSolver.NonlinearSystems
{
    public class BadSystem : ModelSystem
    {
        readonly double sumSinCos;

        public BadSystem(int n) : base(n)
        {
            sumSinCos = Math.Pow(Math.Sin(1), 2) + Math.Pow(Math.Cos(1), 3);
        }

        public override double[] SubstituteValues(double[] x)
        {
            var result = base.SubstituteValues(x);
            result[N - 2] = Math.Pow(Math.Sin(x[0]), 2) + Math.Pow(Math.Cos(x[N - 1]), 3) - sumSinCos;
            return result;
        }

        public override double[,] GetJacobian(double[] x)
        {
            jacobian = base.GetJacobian(x);
            jacobian[N - 2, 0] = -2 * Math.Cos(x[0]);
            for (int i = 1; i < N - 1; i++)
                jacobian[N - 2, i] = 0;
            jacobian[N - 2, N - 1] = 3 * Math.Pow(Math.Sin(x[N - 1]), 2);
            return jacobian;
        }
    }
}
