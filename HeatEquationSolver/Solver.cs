using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Equations;
using HeatEquationSolver.Helpers;
using NLog;
using System;
using System.Linq;
using System.Threading;

namespace HeatEquationSolver
{
	public class Solver
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private ISettings settings;
		private int N;
		private int M;
		private double h;
		private double Tau;
		private double tau;
		private BetaCalculatorBase betaCalculator;
		private HeatEquation equation;

		private readonly double[] x;
		public double[] Answer;
		public double[] PredY;
		public double Norm;
		public CubicSpline CubicSpline;
		public CubicSpline PredCubicSpline;

		public Solver(Settings settings)
		{
			this.settings = settings;
			N = settings.N;
			M = settings.M;
			h = settings.H;
			Tau = settings.Tau;
			betaCalculator = settings.BetaCalculator;
			equation = settings.Equation;

			x = new double[N + 1];
			for (int i = 0; i <= N; i++)
				x[i] = settings.X1 + i * h;

			Logger.Debug("X1={0}, X2={1}, T1={2}, T2={3}, N={4}, M={5}, h={6}, Tau={7}, Epsilon={8}, Beta0={9}, MethodForBeta={10}",
				settings.X1, settings.X2, settings.T1, settings.T2, N, M, h, Tau, settings.Epsilon, settings.Beta0, settings.BetaCalculatorMethod);
		}

		public void Solve(CancellationToken cancellationToken, IProgress<int> progress = null)
		{
			var y0 = new double[N + 1];
			for (int n = 0; n <= N; n++)
				y0[n] = equation.InitCond(x[n]);
			Logger.Debug("Layer 0, y='{0}'", y0.AsString());

			for (int m = 0; m < M; m++)
			{
				progress?.Report(m);
				tau = Tau;
				var yWithPredTau = SolveNonlinearSystem(y0, settings.T1 + m * Tau + Tau);
				int k = 1;
				double norm;
				double[] yWithNextTau;
				do
				{
					cancellationToken.ThrowIfCancellationRequested();
					k *= 2;
					tau = Tau / k;
					double t0 = settings.T1 + m * Tau;
					yWithNextTau = (double[])y0.Clone();
					for (int i = 0; i < k; i++)
						yWithNextTau = SolveNonlinearSystem(yWithNextTau, t0 += tau);
					norm = CalculateNorm(yWithPredTau, yWithNextTau);
					Logger.Debug("norm={0}", norm);
					yWithPredTau = (double[])yWithNextTau.Clone();

				} while (norm > settings.Epsilon2);
				PredY = (double[])y0.Clone();
				y0 = yWithNextTau;
				Logger.Debug("m={0}, tau=Tau/{1}, norm={2}", m, k, norm);
			}

			Answer = y0;
			Norm = 0;

			if (equation.u != null)
			{
				var exactSol = new double[N + 1];
				for (int n = 0; n <= N; n++)
					exactSol[n] = equation.u(x[n], settings.T2);
				Norm = CalculateNorm(y0, exactSol);
			}
			else
			{
				CubicSpline = new CubicSpline();
				CubicSpline.BuildSpline(x, Answer, N + 1);
				PredCubicSpline = new CubicSpline();
				PredCubicSpline.BuildSpline(x, PredY, N + 1);

				double sum = 0;
				foreach (var currentX in x)
				{
					var dx = 1e-5;
					double u = CubicSpline.Interpolate(currentX);
					double du_dt = (u - PredCubicSpline.Interpolate(currentX)) / Tau;
					double du_dx = (CubicSpline.Interpolate(currentX + dx) - u) / dx;
					double d2u_dx2 = ((CubicSpline.Interpolate(currentX + 2 * dx) - CubicSpline.Interpolate(currentX + dx)) / dx - du_dx) / dx;
					var value = equation.SubstituteValues(currentX, settings.T2, u, du_dt, du_dx, d2u_dx2);
					sum += value * value;
				}
				Norm = Math.Sqrt(sum / N);
			}
			Logger.Debug("Answer='{0}', Norm={1}", Answer.AsString(), Norm);
		}

		private double[] SolveNonlinearSystem(double[] y, double t)
		{
			var yK = (double[])y.Clone();
			var f = SubstituteInSystem(t, y, yK);
			Norm = CalculateNorm(f);
			betaCalculator.Init(settings.Beta0, Norm);
			int iterations = 0;

			while (Norm > settings.Epsilon)
			{
				if (++iterations > settings.MaxIterations)
					throw new Exception("Exceeded max number of iterations");

				var answer = ReqularizedMethod(t, y, yK, (double[])f.Clone());

				for (int i = 0; i <= N; i++)
					yK[i] += answer[i];

				f = SubstituteInSystem(t, y, yK);
				Norm = CalculateNorm(f);
				betaCalculator.NextBeta(Norm);
			}

			Logger.Debug("t={0}, {1} iterations, yK='{2}'", t, iterations, yK.AsString());
			return yK;
		}

		private double[] SubstituteInSystem(double t, double[] y, double[] yK)
		{
			var f = new double[N + 1];
			f[0] = yK[0] - equation.LeftBoundCond(t);
			f[N] = yK[N] - equation.RightBoundCond(t);

			for (int n = 1; n < N; n++)
				f[n] = equation.dK_du(x[n], t, y[n]) * Math.Pow((yK[n + 1] - yK[n - 1]) / (2 * h), 2) +
					   equation.K(x[n], t, y[n]) * (yK[n + 1] - 2 * yK[n] + yK[n - 1]) / (h * h) +
					   equation.g(x[n], t, y[n]) - (yK[n] - y[n]) / tau;
			return f;
		}

		private double CalculateNorm(double[] f)
		{
			double sum = f.Sum(e => e * e);
			return Math.Sqrt(sum / N);
		}

		private double CalculateNorm(double[] a, double[] b)
		{
			double sum = 0;
			for (int i = 0; i < a.Length; i++)
				sum += Math.Pow(a[i] - b[i], 2);
			return Math.Sqrt(sum / N);
		}

		private double[] ReqularizedMethod(double t, double[] y, double[] yK, double[] f)
		{
			double alphaBetaNorm = settings.Alpha * betaCalculator.Beta * Norm;
			var jacobian = new double[N + 1, N + 1];     // f'(Xn)
			jacobian[0, 0] = jacobian[N, N] = 1;
			for (int n = 1; n < N; n++)
			{
				double l = equation.dK_du(x[n], t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
				double r = equation.K(x[n], t, y[n]) / (h * h);
				jacobian[n, n - 1] = l + r;
				jacobian[n, n + 1] = -l + r;
				jacobian[n, n] = -2 * r - 1 / tau;
			}

			var a = jacobian.Transpose();
			a = a.AddDiag(alphaBetaNorm);
			var matrix = a.Multiply(jacobian).AddDiag(alphaBetaNorm);
			var freeMembers = a.Multiply(f).MultiplyConst(betaCalculator.Multiplier);
			return ResolvingSystem.Gauss(matrix, freeMembers);
		}

		#region Unused

		private double[] MakeAndSolveSystem(double t, double[] y, double[] yK, double[] f)
		{
			var a = new double[N + 1];
			var c = new double[N + 1];
			var b = new double[N + 1];
			c[0] = c[N] = 1;

			for (int n = 1; n < N; n++)
			{
				double l = equation.dK_du(x[n], t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
				double r = equation.K(x[n], t, y[n]) / (h * h);
				a[n - 1] = l + r;       // or a[n],b[n]?
				b[n - 1] = -l + r;
				c[n] = -2 * r - 1 / tau;
				f[n] *= betaCalculator.Multiplier;
			}
			f[0] *= betaCalculator.Multiplier;
			f[N] *= betaCalculator.Multiplier;

			return ResolvingSystem.TridiagonalMatrixAlgorithm(a, c, b, f);
		}

		private double[] FullMatrix(double t, double[] y, double[] yK, double[] f)
		{
			var jacobian = new double[N + 1, N + 1];     // f'(Xn)
			jacobian[0, 0] = jacobian[N, N] = 1;
			for (int n = 1; n < N; n++)
			{
				double l = equation.dK_du(x[n], t, y[n]) * (yK[n - 1] - yK[n + 1]) / (2 * h * h);
				double r = equation.K(x[n], t, y[n]) / (h * h);
				jacobian[n, n - 1] = l + r;
				jacobian[n, n + 1] = -l + r;
				jacobian[n, n] = -2 * r - 1 / tau;
				f[n] *= betaCalculator.Multiplier;
			}
			f[0] *= betaCalculator.Multiplier;
			f[N] *= betaCalculator.Multiplier;
			return ResolvingSystem.Gauss(jacobian, f);
		}

		#endregion
	}
}
