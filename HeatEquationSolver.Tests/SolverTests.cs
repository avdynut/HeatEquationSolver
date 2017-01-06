using System;
using HeatEquationSolver.Samples;
using NUnit.Framework;
using System.Threading;

namespace HeatEquationSolver.Tests
{
    public class SolverTests
    {
        private Solver solver;

        [OneTimeSetUp]
        public void Init()
        {
            var equation = new HeatEquation(OldEquation.u, OldEquation.K, OldEquation.g, OldEquation.dK_du, OldEquation.InitCond, OldEquation.LeftBoundCond, OldEquation.RightBoundCond);
			equation.CheckEquation(OldEquation.du_dx, OldEquation.d2u_dx2, OldEquation.du_dt);
            Settings.Equation = equation;
            solver = new Solver();
            solver.Solve(new CancellationToken());
        }

        [Test]
        public void Solver_should_return_correct_answer()
        {
            string expectedAnswer = "6.53750503519499E-19\t0\r\n0.310285791414608\t0.31\r\n0.640156400221993\t0.64\r\n0.990127783887749\t0.99\r\n1.36010375138145\t1.36\r\n1.75008245023215\t1.75\r\n2.16006317990873\t2.16\r\n2.59004553577665\t2.59\r\n3.04002924712485\t3.04\r\n3.51001411820152\t3.51\r\n4.00000000000001\t4\r\n";
            Assert.That(solver.Answer, Is.EqualTo(expectedAnswer), "Incorrect answer");
        }

        [Test]
        public void Solver_should_return_correct_norm()
        {
            double expectedNorm = 0.0003836d;
            Assert.That(Math.Round(solver.Norm, 7), Is.EqualTo(expectedNorm), "Incorrect norm");
        }
    }
}
