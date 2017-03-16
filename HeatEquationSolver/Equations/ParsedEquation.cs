using org.mariuszgromada.math.mxparser;

namespace HeatEquationSolver.Equations
{
    public class ParsedEquation : HeatEquation
    {
        public string _K { get; set; }
        private ComplexFunction k;
        public override ComplexFunction K => k;

        public string _dK_du { get; set; }
        private ComplexFunction dK_duFunc;
        public override ComplexFunction dK_du => dK_duFunc;

        public string _g { get; set; }
        private ComplexFunction gFunc;
        public override ComplexFunction g => gFunc;

        public string _InitCond { get; set; }
        private InitialCondition initCond;
        public override InitialCondition InitCond => initCond;

        public string _LeftBoundCond { get; set; }
        private BoundaryCondtion leftBoundCond;
        public override BoundaryCondtion LeftBoundCond => leftBoundCond;

        public string _RightBoundCond { get; set; }
        private BoundaryCondtion rightBoundCond;
        public override BoundaryCondtion RightBoundCond => rightBoundCond;

        #region Optional

        public string _u { get; set; }
        private Function uFunc;
        public override Function u => uFunc;

        public string _du_dx { get; set; }
        private Function du_dxFunc;
        public override Function du_dx => du_dxFunc;

        public string _d2u_dx2 { get; set; }
        private Function d2u_dx2Func;
        public override Function d2u_dx2 => d2u_dx2Func;

        public string _du_dt { get; set; }
        private Function du_dtFunc;
        public override Function du_dt => du_dtFunc;

        #endregion

        public void ParseFunctions()
        {
            k = (x, t, u) => new Expression(_K, new Argument("x", x), new Argument("t", t), new Argument("u", u)).calculate();
            dK_duFunc = (x, t, u) => new Expression(_dK_du, new Argument("x", x), new Argument("t", t), new Argument("u", u)).calculate();
            gFunc = (x, t, u) => new Expression(_g, new Argument("x", x), new Argument("t", t), new Argument("u", u), new Argument("K", K(x, t, u))).calculate();
            initCond = x => new Expression(_InitCond, new Argument("x", x)).calculate();
            leftBoundCond = t => new Expression(_LeftBoundCond, new Argument("t", t)).calculate();
            rightBoundCond = t => new Expression(_RightBoundCond, new Argument("t", t)).calculate();

            if (!string.IsNullOrEmpty(_u))
                uFunc = (x, t) => new Expression(_u, new Argument("x", x), new Argument("t", t)).calculate();
            if (!string.IsNullOrEmpty(_du_dx))
                du_dxFunc = (x, t) => new Expression(_du_dx, new Argument("x", x), new Argument("t", t)).calculate();
            if (!string.IsNullOrEmpty(_d2u_dx2))
                d2u_dx2Func = (x, t) => new Expression(_d2u_dx2, new Argument("x", x), new Argument("t", t)).calculate();
            if (!string.IsNullOrEmpty(_du_dt))
                du_dtFunc = (x, t) => new Expression(_du_dt, new Argument("x", x), new Argument("t", t)).calculate();
        }
    }
}
