using static HeatEquationSolver.EntryPoint;

namespace HeatEquationSolver.SystemWorker
{
    public class Regularized : SystemWorker
    {
        protected override void ComposeSystem()
        {
            double alphaBetaNorm = alpha * BetaCalculator.Beta * norm;
            double[,] jacobian = system.GetJacobian(x);     // f'(xn)
            var a = Matrix.Transpose(jacobian);             // transposed f'(xn) 
            a = Matrix.AddDiag(a, alphaBetaNorm);
            matrix = Matrix.AddDiag(Matrix.Multiply(a, jacobian), alphaBetaNorm);
            freeMembers = Vector.MultiplyConst(CoefficientBeta(), Matrix.Multiply(a, f));
        }

        protected override void SolveSystem()
        {
            Answer = ResolvingSystem.Gauss();
        }
    }
}
