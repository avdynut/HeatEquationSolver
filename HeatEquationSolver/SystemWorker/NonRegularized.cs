using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver.SystemWorker
{
    public class NonRegularized : SystemWorker
    {
        private double[] a;
        private double[] c;
        private double[] b;

        public NonRegularized()
        {
            a = new double[N + 1];
            c = new double[N + 1];
            b = new double[N + 1];
            c[0] = c[N] = 1;
        }

        protected override void ComposeSystem()
        {
            for (int n = 1; n < N; n++)
            {
                double x = n * h;
                double l = equation.dK_dy(x, t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
                double r = equation.K(x, t, y[n]) / (h * h);
                a[n - 1] = l + r;
                b[n - 1] = -l + r;
                c[n] = -2 * r - 1 / tau;
                //r = -K(x, t, y[n]) / (h * h);
                //A[n - 1] = -l + r;
                //B[n - 1] = l + r;
                //C[n] = -2 * r + 1 / tau;
                f[n] *= BetaCalculator.Multiplier;
            }
            f[0] *= BetaCalculator.Multiplier;
            f[N] *= BetaCalculator.Multiplier;
        }

        protected override void SolveSystem()
        {
            Answer = TridiagonalMatrixAlgorithm(a, c, b, f);
        }

        private double[] TridiagonalMatrixAlgorithm(double[] a, double[] c, double[] b, double[] f)
        {
            int n = c.Length;
            var x = new double[n];

            for (int i = 1; i < n; i++)
            {
                double m = a[i] / c[i - 1];
                c[i] -= m * b[i - 1];
                f[i] -= m * f[i - 1];
            }

            x[n - 1] = f[n - 1] / c[n - 1];
            for (int i = n - 2; i >= 0; i--)
                x[i] = (f[i] - b[i] * x[i + 1]) / c[i];

            return x;
        }
    }
}
