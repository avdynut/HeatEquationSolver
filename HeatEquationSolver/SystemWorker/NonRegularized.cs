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
                f[n] *= BetaCalculator.Multiplier;
            }
            f[0] *= BetaCalculator.Multiplier;
            f[N] *= BetaCalculator.Multiplier;
        }

        protected override void SolveSystem()
        {
            Answer = ResolvingSystem.TridiagonalMatrixAlgorithm(a, c, b, f);
        }
    }
}
