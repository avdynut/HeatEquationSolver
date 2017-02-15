using System;
using NUnit.Framework;
using System.Threading;
using System.Linq;

namespace HeatEquationSolver.Tests
{
    public class SolverTests
    {
        private Solver solver;
        private HeatEquation equation;

        [OneTimeSetUp]
        public void Init()
        {
            equation = new HeatEquation(ModelEquation.K, ModelEquation.dK_du, ModelEquation.g, ModelEquation.InitCond, ModelEquation.LeftBoundCond, ModelEquation.RightBoundCond)
            {
                u = ModelEquation.u
            };
            Settings.Equation = equation;
            solver = new Solver();
            solver.Solve(new CancellationToken());
        }

        [Test]
        public void CheckModelEquation()
        {
            Assert.That(equation.IsEquationCorrect(ModelEquation.du_dx, ModelEquation.d2u_dx2, ModelEquation.du_dt), "Incorrect equation");
        }

        [Test]
        public void SolverShouldFindCorrectApporximatedValues()
        {

            double[] expectedValues = { 0, 0.3102857914, 0.6401564002, 0.9901277839, 1.3601037514, 1.7500824502, 2.1600631799, 2.5900455358, 3.0400292471, 3.5100141182, 4 };
            var approximatedValues = solver.Answer.Select(x => Math.Round(x, 10));
            Assert.That(approximatedValues, Is.EqualTo(expectedValues), "Incorrect answer");
        }

        [Test]
        public void ApproximatedValuesShouldResembleToExactValues()
        {
            var exactValues = new double[Settings.N + 1];
            for (int i = 0; i <= Settings.N; i++)
                exactValues[i] = Math.Round(equation.u(Settings.X1 + i * Settings.H, Settings.T2), 3);
            var approximatedValues = solver.Answer.Select(x => Math.Round(x, 3));
            Assert.That(approximatedValues, Is.EqualTo(exactValues), "Incorrect answer");
        }

        [Test]
        public void SolverShouldReturnCorrectNorm()
        {
            double expectedNorm = 0.000121;
            Assert.That(Math.Round(solver.Norm, 6), Is.EqualTo(expectedNorm), "Incorrect norm");
        }
    }
}
