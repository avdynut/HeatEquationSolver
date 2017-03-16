using SimpleExpressionEvaluator;

namespace HeatEquationSolver.Equations
{
    public class ParsedEquation : HeatEquation
    {
        private ExpressionEvaluator ev = new ExpressionEvaluator();

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
            k = (x, t, u) => (double)ev.Evaluate(_K, new { x, t, u });
            dK_duFunc = (x, t, u) => (double)ev.Evaluate(_dK_du, new { u });
            gFunc = (x, t, u) =>
            {
                var K = k(x, t, u);
                return (double)ev.Evaluate(_g, new { x, t, u, K });
            };
            initCond = x => (double)ev.Evaluate(_InitCond);
            leftBoundCond = t => (double)ev.Evaluate(_LeftBoundCond);
            rightBoundCond = t => (double)ev.Evaluate(_RightBoundCond, new { t });

            if (!string.IsNullOrEmpty(_u))
                uFunc = (x, t) => (double)ev.Evaluate(_u, new { x, t });
            if (!string.IsNullOrEmpty(_du_dx))
                du_dxFunc = (x, t) => (double)ev.Evaluate(_du_dx, new { x, t });
            if (!string.IsNullOrEmpty(_d2u_dx2))
                d2u_dx2Func = (x, t) => (double)ev.Evaluate(_d2u_dx2, new { t });
            if (!string.IsNullOrEmpty(_du_dt))
                du_dtFunc = (x, t) => (double)ev.Evaluate(_du_dt, new { x, t });
        }
    }
}
