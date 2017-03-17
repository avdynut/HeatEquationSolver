using HeatEquationSolver.Equations;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace HeatEquationSolver.Tests
{
    public class ParsedEquationTests
    {
        private ParsedEquation equation;
        private ModelEquation modelEq;

        private Random r = new Random();
        private double NextRandomDouble => r.Next(-10, 10) + r.NextDouble();

        [OneTimeSetUp]
        public void Init()
        {
            equation = new ParsedEquation
            {
                _K = ConfigHelper.K,
                _dK_du = ConfigHelper.dK_du,
                _g = ConfigHelper.g,
                _InitCond = ConfigHelper.InitCond,
                _LeftBoundCond = ConfigHelper.LeftBoundCond,
                _RightBoundCond = ConfigHelper.RightBoundCond,
                _u = ConfigHelper.u,
                _du_dx = ConfigHelper.du_dx,
                _d2u_dx2 = ConfigHelper.d2u_dx2,
                _du_dt = ConfigHelper.du_dt
            };
            equation.ParseFunctions();
            Settings.Equation = equation;
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
            var pSolver = new Solver();
            pSolver.Solve(new CancellationToken());

            var mSolver = new Solver();
            Settings.Equation = modelEq;
            mSolver.Solve(new CancellationToken());

            Assert.That(pSolver.Norm.Round(), Is.EqualTo(mSolver.Norm.Round()), "Incorrect norm");
            Assert.That(pSolver.Answer.Select(a => a.Round()), Is.EqualTo(mSolver.Answer.Select(a => a.Round())), "Incorrect answer");
        }
    }

    public static class MathHelper
    {
        public static double Round(this double a)
        {
            //return a;
            return Math.Round(a, 8);
        }
    }
}
