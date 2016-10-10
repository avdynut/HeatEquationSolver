namespace HeatEquationSolver.SystemWorker
{
    public abstract class SystemWorker
    {
        protected double[] Answer;
        protected double[] y;
        protected double[] yK;
        protected double[] f;
        protected double t;

        protected abstract void ComposeSystem();

        protected abstract void SolveSystem();

        public double[] MakeAndSolveSystem(double t, double[] y, double[] yK, double[] f)
        {
            this.t = t;
            this.y = y;
            this.yK = yK;
            this.f = f;
            ComposeSystem();
            SolveSystem();
            return Answer;
        }
    }
}
