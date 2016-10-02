using System;
using System.Linq;

namespace NonlinearSystemSolver.NonlinearSystems
{
    public class ModelSystem : NonlinearSystem
    {
        protected double[,] jacobian;

        public ModelSystem(int n) : base(n)
        {
            GenerateInvariablePartJacobian();
        }

        public override double[] SubstituteValues(double[] x)
        {
            if (x.Length != N)
                throw new ArgumentException("Incorrect vector x");

            double[] result = new double[N];
            for (int i = 0; i < N - 1; i++)
                result[i] = x.Sum() + x[i] - N - 1;

            // последняя строка
            double lastValue = x[0];
            for (int i = 1; i < N; i++)
                lastValue *= x[i];
            result[N - 1] = lastValue - 1;

            return result;
        }

        private void GenerateInvariablePartJacobian()
        {
            jacobian = new double[N, N];
            for (int i = 0; i < N - 1; i++)
            {
                for (int j = 0; j < N; j++)
                    jacobian[i, j] = 1;
                jacobian[i, i] = 2;
            }
        }

        public override double[,] GetJacobian(double[] x)
        {
            for (int i = 0; i < N; i++)
            {
                jacobian[N - 1, i] = 1;
                for (int j = 0; j < N; j++)
                    if (i != j)
                        jacobian[N - 1, i] *= x[j];
            }
            return jacobian;
        }
    }
}
