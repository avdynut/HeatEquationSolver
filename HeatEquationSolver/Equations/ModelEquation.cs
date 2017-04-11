using System;

namespace HeatEquationSolver.Equations
{
	public class ModelEquation : HeatEquation
	{
		public override ComplexFunction K => (x, t, u) => x * x * t + u * u;

		public override ComplexFunction dK_du => (x, t, u) => 2 * u;

		public override ComplexFunction g => (x, t, u) => x * x + 6 * x * t - 2 * u * Math.Pow(2 * x * t + 3 * t * t, 2) - 2 * t * (x * x * t + u * u);

		public override InitialCondition InitCond => x => 0;

		public override BoundaryCondtion LeftBoundCond => t => 0;

		public override BoundaryCondtion RightBoundCond => t => t + 3 * t * t;


		public override Function u => (x, t) => x * x * t + 3 * x * t * t;

		public override Function du_dx => (x, t) => 2 * x * t + 3 * t * t;

		public override Function d2u_dx2 => (x, t) => 2 * t;

		public override Function du_dt => (x, t) => x * x + 6 * x * t;
	}
}
