using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Equations;
using HeatEquationSolver.Helpers;
using HeatEquationSolver.Settings;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HeatEquationSolver
{
	public class Solver
	{
		public delegate void LayersQuantityHandler(int m);
		public event LayersQuantityHandler ChangedNumberOfLayers;

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private ISettings settings;
		private int N;
		private double h;
		private double tau;
		private BetaCalculatorBase betaCalculator;
		private HeatEquation equation;
		private List<double> iterationsOnLayers;

		private readonly double[] x;
		public double[] Answer;
		public double Norm;
		public double AverageIterationsOnLayer => iterationsOnLayers.Average();

		private BetaCalculatorBase GetBetaCalculator(BetaCalculator betaCalculatorMethod)
		{
			switch (betaCalculatorMethod)
			{
				case BetaCalculator.Puzynin:
					return new PuzyninMethod();
				case BetaCalculator.Osmoip_1_5:
					return new Osmoip_1_5();
				case BetaCalculator.Osmoip_1_47:
					return new Osmoip_1_47();
				case BetaCalculator.Osmoip_1_267:
					return new Osmoip_1_267();
				case BetaCalculator.Osmoip_1_274:
					return new Osmoip_1_274();
				case BetaCalculator.Osmoip_1_301:
					return new Osmoip_1_301();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Solver(ISettings settings)
		{
			this.settings = settings;
			N = settings.N;
			h = (settings.X2 - settings.X1) / N;
			tau = (settings.T2 - settings.T1) / settings.M;
			betaCalculator = GetBetaCalculator(settings.BetaCalculatorMethod);
			equation = settings.UseParsedEquation ? new ParsedEquation(settings.Functions) : (HeatEquation)new ModelEquation();

			x = new double[N + 1];
			for (int i = 0; i <= N; i++)
				x[i] = settings.X1 + i * h;

			Logger.Debug("X1={0}, X2={1}, T1={2}, T2={3}, N={4}, M={5}, h={6}, Tau={7}, Epsilon={8}, Beta0={9}, MethodForBeta={10}",
				settings.X1, settings.X2, settings.T1, settings.T2, N, settings.M, h, tau, settings.Epsilon, settings.Beta0, settings.BetaCalculatorMethod);
		}

		public void Solve(CancellationToken cancellationToken = new CancellationToken(), IProgress<int> progress = null)
		{
			iterationsOnLayers = new List<double>();

			var y = new double[N + 1];
			for (int n = 0; n <= N; n++)
				y[n] = equation.InitCond(x[n]);
			Logger.Debug("Layer 0, y='{0}'", y.AsString());

			progress?.Report(1);
			FindOptimalTau(y, settings.T1, cancellationToken);
			settings.M = (int)((settings.T2 - settings.T1) / tau);
			ChangedNumberOfLayers?.Invoke(settings.M);

			for (int m = 1; m < settings.M; m++)
			{
				cancellationToken.ThrowIfCancellationRequested();
				progress?.Report(m);
				y = SolveNonlinearSystem(y, settings.T1 + m * tau);
			}

			double t = settings.T2 - tau;
			FindOptimalTau(y, t, cancellationToken);
			progress?.Report(settings.M);
			while (t != settings.T2)
			{
				t += tau;
				y = SolveNonlinearSystem(y, t);
			}

			Answer = y;
			Norm = 0;
			if (equation.u == null)
				Norm = settings.Epsilon2;
			else
			{
				var exactSol = new double[N + 1];
				for (int n = 0; n <= N; n++)
					exactSol[n] = equation.u(x[n], settings.T2);
				Norm = CalculateNorm(y, exactSol);
			}

			Logger.Debug("Answer='{0}', Norm={1}", Answer.AsString(), Norm);
		}

		private void FindOptimalTau(double[] yPred, double tPred, CancellationToken cancellationToken)
		{
			int k = 1;
			double norm;
			var yNext = SolveNonlinearSystem(yPred, tPred + tau);
			do
			{
				cancellationToken.ThrowIfCancellationRequested();
				k *= 2;
				tau /= 2;
				var y = (double[])yPred.Clone();
				for (int i = 1; i <= k; i++)
					y = SolveNonlinearSystem(y, tPred + i * tau);
				norm = CalculateNorm(yNext, y);
				yNext = y;
				Logger.Debug("norm={0}", norm);
			} while (norm > settings.Epsilon2);
			tau *= 2;
		}

		private double[] SolveNonlinearSystem(double[] y, double t)
		{
			var yK = (double[])y.Clone();
			var f = SubstituteInSystem(t, y, yK);
			Norm = CalculateNorm(f);
			betaCalculator.Init(settings.Beta0, Norm);
			int iterations = 0;

			while (Norm >= settings.Epsilon)
			{
				if (++iterations > settings.MaxIterations)
					throw new Exception("Exceeded max number of iterations");

				var answer = ReqularizedMethod(t, y, yK, (double[])f.Clone());

				for (int i = 0; i <= N; i++)
					yK[i] += answer[i];

				f = SubstituteInSystem(t, y, yK);
				Norm = CalculateNorm(f);
				betaCalculator.CalculateNextBeta(Norm);
			}

			iterationsOnLayers.Add(iterations);
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
