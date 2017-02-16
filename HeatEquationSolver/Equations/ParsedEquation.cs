using System;
using Z.Expressions;

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
            k = (x, t, u) => Eval.Compile<Func<double, double, double, double>>(_K, "x", "t", "u")(x, t, u);
            dK_duFunc = (x, t, u) => Eval.Compile<Func<double, double, double, double>>(_dK_du, "x", "t", "u")(x, t, u);
            gFunc = (x, t, u) => Eval.Compile<Func<double, double, double, double, double>>(_g, "x", "t", "u", "K")(x, t, u, K(x, t, u));
            initCond = x => Eval.Compile<Func<double, double>>(_InitCond, "x")(x);
            leftBoundCond = t => Eval.Compile<Func<double, double>>(_LeftBoundCond, "t")(t);
            rightBoundCond = t => Eval.Compile<Func<double, double>>(_RightBoundCond, "t")(t);

            if (!string.IsNullOrEmpty(_u))
                uFunc = (x, t) => Eval.Compile<Func<double, double, double>>(_u, "x", "t")(x, t);
            if (!string.IsNullOrEmpty(_du_dx))
                du_dxFunc = (x, t) => Eval.Compile<Func<double, double, double>>(_du_dx, "x", "t")(x, t);
            if (!string.IsNullOrEmpty(_d2u_dx2))
                d2u_dx2Func = (x, t) => Eval.Compile<Func<double, double, double>>(_d2u_dx2, "x", "t")(x, t);
            if (!string.IsNullOrEmpty(_du_dt))
                du_dtFunc = (x, t) => Eval.Compile<Func<double, double, double>>(_du_dt, "x", "t")(x, t);
        }
    }
}
