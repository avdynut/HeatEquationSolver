using HeatEquationSolver;
using HeatEquationSolver.Equations;

namespace HeatEquationSolverUI
{
    public class Equation
    {
        public ParsedEquation ParsedEq { get; }
        public string K { get { return ParsedEq._K; } set { ParsedEq._K = value; } }
        public string dK_du { get { return ParsedEq._dK_du; } set { ParsedEq._dK_du = value; } }
        public string g { get { return ParsedEq._g; } set { ParsedEq._g = value; } }
        public string InitCond { get { return ParsedEq._InitCond; } set { ParsedEq._InitCond = value; } }
        public string LeftBoundCond { get { return ParsedEq._LeftBoundCond; } set { ParsedEq._LeftBoundCond = value; } }
        public string RightBoundCond { get { return ParsedEq._RightBoundCond; } set { ParsedEq._RightBoundCond = value; } }
        public string u { get { return ParsedEq._u; } set { ParsedEq._u = value; } }
        //public string du_dx { get { return eq._du_dx; } set { eq._du_dx = value; } }
        //public string d2u_dx2 { get { return eq._d2u_dx2; } set { eq._d2u_dx2 = value; } }
        //public string du_dt { get { return eq._du_dt; } set { eq._du_dt = value; } }

        public Equation()
        {
            ParsedEq = new ParsedEquation();
            K = ConfigHelper.K;
            dK_du = ConfigHelper.dK_du;
            g = ConfigHelper.g;
            InitCond = ConfigHelper.InitCond;
            LeftBoundCond = ConfigHelper.LeftBoundCond;
            RightBoundCond = ConfigHelper.RightBoundCond;
            u = ConfigHelper.u;
        }
    }
}
