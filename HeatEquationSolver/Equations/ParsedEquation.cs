using Mathos.Parser;

namespace HeatEquationSolver.Equations
{
    public class ParsedEquation : HeatEquation
    {
        private MathParser parser = new MathParser();

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
            k = (x, t, u) => ParseComplexFunc(_K, x, t, u);
            dK_duFunc = (x, t, u) => ParseComplexFunc(_dK_du, x, t, u);
            gFunc = (x, t, u) => ParseG(_g, x, t, u, K(x, t, u));
            initCond = x => ParseCond(_InitCond, "x", x);
            leftBoundCond = t => ParseCond(_LeftBoundCond, "t", t);
            rightBoundCond = t => ParseCond(_RightBoundCond, "t", t);

            if (!string.IsNullOrEmpty(_u))
                uFunc = (x, t) => ParseFunc(_u, x, t);
            if (!string.IsNullOrEmpty(_du_dx))
                du_dxFunc = (x, t) => ParseFunc(_du_dx, x, t);
            if (!string.IsNullOrEmpty(_d2u_dx2))
                d2u_dx2Func = (x, t) => ParseFunc(_d2u_dx2, x, t);
            if (!string.IsNullOrEmpty(_du_dt))
                du_dtFunc = (x, t) => ParseFunc(_du_dt, x, t);
        }

        private double ParseG(string function, double x, double t, double u, double K)
        {
            parser.LocalVariables["K"] = (decimal)K;
            return ParseComplexFunc(function, x, t, u);
        }

        private double ParseComplexFunc(string function, double x, double t, double u)
        {
            parser.LocalVariables["u"] = (decimal)u;
            return ParseFunc(function, x, t);
        }

        private double ParseFunc(string function, double x, double t)
        {
            parser.LocalVariables["x"] = (decimal)x;
            parser.LocalVariables["t"] = (decimal)t;
            return Parse(function);
        }

        private double ParseCond(string function, string variable, double a)
        {
            parser.LocalVariables[variable] = (decimal)a;
            return Parse(function);
        }

        private double Parse(string function)
        {
            return (double)parser.ProgrammaticallyParse(function);
        }
    }
}
