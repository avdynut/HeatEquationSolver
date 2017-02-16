using HeatEquationSolver.Equations;
using NUnit.Framework;
using System;

namespace HeatEquationSolver.Tests
{
    public class ParsedEquationTests
    {
        private ParsedEquation equation;

        private Random r = new Random();
        private double NextRandomDouble => r.Next(-10, 10) + r.NextDouble();

        [OneTimeSetUp]
        public void Init()
        {
            equation = new ParsedEquation();
            Settings.Equation = equation;
        }

        [Test]
        public void CheckCorrectParsingFunctions()
        {
            var modelEq = new ModelEquation();
            equation._K = ResourcesHelper.K;
            equation._dK_du = ResourcesHelper.dK_du;
            equation._g = ResourcesHelper.g;
            equation._InitCond = ResourcesHelper.InitCond;
            equation._LeftBoundCond = ResourcesHelper.LeftBoundCond;
            equation._RightBoundCond = ResourcesHelper.RightBoundCond;
            equation._u = ResourcesHelper.u;
            equation._du_dx = ResourcesHelper.du_dx;
            equation._d2u_dx2 = ResourcesHelper.d2u_dx2;
            equation._du_dt = ResourcesHelper.du_dt;
            equation.ParseFunctions();

            double x = NextRandomDouble;
            double t = NextRandomDouble;
            double u = equation.u(x, t);
            Assert.That(u, Is.EqualTo(modelEq.u(x, t)));
            Assert.That(equation.K(x, t, u), Is.EqualTo(modelEq.K(x, t, u)));
            Assert.That(equation.dK_du(x, t, u), Is.EqualTo(modelEq.dK_du(x, t, u)));
            Assert.That(equation.g(x, t, u), Is.EqualTo(modelEq.g(x, t, u)));
            Assert.That(equation.InitCond(x), Is.EqualTo(modelEq.InitCond(x)));
            Assert.That(equation.LeftBoundCond(t), Is.EqualTo(modelEq.LeftBoundCond(t)));
            Assert.That(equation.RightBoundCond(t), Is.EqualTo(modelEq.RightBoundCond(t)));
            Assert.That(equation.du_dx(x, t), Is.EqualTo(modelEq.du_dx(x, t)));
            Assert.That(equation.d2u_dx2(x, t), Is.EqualTo(modelEq.d2u_dx2(x, t)));
            Assert.That(equation.du_dt(x, t), Is.EqualTo(modelEq.du_dt(x, t)));
        }
    }
}
