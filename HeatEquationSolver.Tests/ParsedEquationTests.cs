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
            equation._K = ConfigHelper.K;
            equation._dK_du = ConfigHelper.dK_du;
            equation._g = ConfigHelper.g;
            equation._InitCond = ConfigHelper.InitCond;
            equation._LeftBoundCond = ConfigHelper.LeftBoundCond;
            equation._RightBoundCond = ConfigHelper.RightBoundCond;
            equation._u = ConfigHelper.u;
            equation._du_dx = ConfigHelper.du_dx;
            equation._d2u_dx2 = ConfigHelper.d2u_dx2;
            equation._du_dt = ConfigHelper.du_dt;
            equation.ParseFunctions();

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
