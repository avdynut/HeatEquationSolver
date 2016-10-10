namespace HeatEquationSolver.Samples
{
    public static class CurrentEq
    {
        public static double u(double x, double t)
        {
            return x * x * t;
        }

        public static double g(double x, double t, double u)
        {
            return x * x - 8 * x * x * t * t * u - 2 * t * u * u;
        }

        public static double K(double x, double t, double u)
        {
            return u * u;
        }

        public static double KDy(double x, double t, double y)
        {
            return 2 * y;
        }

        public static double du_dx(double x, double t)
        {
            return 2 * x * t;
        }

        public static double d2u_dx2(double x, double t)
        {
            return 2 * t;
        }

        public static double du_dt(double x, double t)
        {
            return x * x;
        }
    }
}
