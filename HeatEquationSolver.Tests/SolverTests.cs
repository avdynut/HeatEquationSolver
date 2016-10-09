using NUnit.Framework;

namespace HeatEquationSolver.Tests
{
    public class SolverTests
    {
        private QuasiNewton solver;
        //private Solver solver;

        [OneTimeSetUp]
        public void Init()
        {
            solver = new QuasiNewton(1, 10, 1, 400, 0.01, 1e-3);
            solver.Solve();
            //var equation = new Equation(QuasiNewton.u, QuasiNewton.K, QuasiNewton.g, QuasiNewton.KDy);
            //EntryPoint.equation = equation;
            //EntryPoint.SetUp(NonlinearSystemSolver.BetaCalculators.MethodBeta.Puzynin);
            //solver = new Solver();
            //solver.Solve();
        }

        [Test]
        public void Solver_should_return_correct_answer()
        {
            string expectedAnswer = "-0,000176932486518374\t0\r\n0,310320509963102\t0,31\r\n0,640131473058538\t0,64\r\n0,990104944381454\t0,99\r\n1,3600843109976\t1,36\r\n1,75006662489554\t1,75\r\n2,16005088535975\t2,16\r\n2,59003659406273\t2,59\r\n3,04002346526933\t3,04\r\n3,51001131370279\t3,51\r\n4\t4\r\n";
            Assert.That(solver.Answer, Is.EqualTo(expectedAnswer), "Incorrect answer");
        }

        [Test]
        public void Solver_should_return_correct_norm()
        {
            double expectedNorm = 0.00042247427682090658;
            Assert.That(solver.Norm, Is.EqualTo(expectedNorm), "Incorrect norm");
        }
    }
}
