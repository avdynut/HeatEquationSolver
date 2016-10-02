namespace NonlinearSystemSolver.NonlinearSystems
{
    public abstract class NonlinearSystem
    {
        public int N { get; private set; }

        protected NonlinearSystem(int n)
        {
            N = n;
        }

        public abstract double[] SubstituteValues(double[] x);

        public abstract double[,] GetJacobian(double[] x);

        //public static double NormL2(double[] f)
        //{
        //    double sum = 0;
        //    for (int i = 0; i < f.Length; i++)
        //        sum += Math.Pow(f[i], 2);
        //    return Math.Sqrt(sum);
        //}
    }
}
