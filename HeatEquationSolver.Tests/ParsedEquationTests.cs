using HeatEquationSolver.Equations;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace HeatEquationSolver.Tests
{
	public class ParsedEquationTests
	{
		private Settings settings;
		private ParsedEquation equation;
		private ModelEquation modelEq;

		private Random r = new Random();
		private double NextRandomDouble => r.Next(-10, 10) + r.NextDouble();

		[OneTimeSetUp]
		public void Init()
		{
			settings = DataManager.Settings;
			equation = (ParsedEquation)settings.Equation;
			modelEq = new ModelEquation();
		}

		[Test]
		public void CheckCorrectParsingFunctions()
		{
			double x = NextRandomDouble;
			double t = NextRandomDouble;
			double u = equation.u(x, t);
			Assert.That(u.Round(), Is.EqualTo(modelEq.u(x, t).Round()));
			Assert.That(equation.K(x, t, u).Round(), Is.EqualTo(modelEq.K(x, t, u).Round()));
			Assert.That(equation.dK_du(x, t, u).Round(), Is.EqualTo(modelEq.dK_du(x, t, u).Round()));
			Assert.That(equation.g(x, t, u).Round(), Is.EqualTo(modelEq.g(x, t, u).Round()));
			Assert.That(equation.InitCond(x).Round(), Is.EqualTo(modelEq.InitCond(x).Round()));
			Assert.That(equation.LeftBoundCond(t).Round(), Is.EqualTo(modelEq.LeftBoundCond(t).Round()));
			Assert.That(equation.RightBoundCond(t).Round(), Is.EqualTo(modelEq.RightBoundCond(t).Round()));
			Assert.That(equation.du_dx(x, t).Round(), Is.EqualTo(modelEq.du_dx(x, t).Round()));
			Assert.That(equation.d2u_dx2(x, t).Round(), Is.EqualTo(modelEq.d2u_dx2(x, t).Round()));
			Assert.That(equation.du_dt(x, t).Round(), Is.EqualTo(modelEq.du_dt(x, t).Round()));
		}

		[Test]
		public void CheckSolution()
		{
			var pSolver = new Solver(settings);
			pSolver.Solve(new CancellationToken());

			settings.UseParsedEquation = false;
			var mSolver = new Solver(settings);
			mSolver.Solve(new CancellationToken());

			Assert.That(pSolver.Norm.Round(), Is.EqualTo(mSolver.Norm.Round()), "Incorrect norm");
			Assert.That(pSolver.Answer.Select(a => a.Round()), Is.EqualTo(mSolver.Answer.Select(a => a.Round())), "Incorrect answer");
		}

		[Test]
		public void SubstituteSolutionInEquation()
		{
			settings.UseParsedEquation = true;
			settings.Functions.u = "";
			var pSolver = new Solver(settings);
			pSolver.Solve(new CancellationToken());

			var dx = 1e-5;
			double x = settings.X1 + (settings.X2 - settings.X1) / 2;
			double t = settings.T2;
			double u = pSolver.CubicSpline.Interpolate(x);
			double du_dt = (u - pSolver.PredCubicSpline.Interpolate(x)) / settings.Tau;
			double du_dx = (pSolver.CubicSpline.Interpolate(x + dx) - u) / dx;
			double d2u_dx2 = ((pSolver.CubicSpline.Interpolate(x + 2 * dx) - pSolver.CubicSpline.Interpolate(x + dx)) / dx - du_dx) / dx;
			var actualValue = settings.Equation.SubstituteValues(x, t, u, du_dt, du_dx, d2u_dx2);
			Assert.That(actualValue.Round(0), Is.EqualTo(0), "Incorrect solution");
		}
	}

	public static class MathHelper
	{
		public static double Round(this double a, int decimals = 8)
		{
			//return a;
			return Math.Round(a, decimals);
		}
	}
}
