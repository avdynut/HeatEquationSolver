using HeatEquationSolver.Settings;
using HeatEquationSolverUI.Base;

namespace HeatEquationSolverUI
{
	public class FunctionsViewModel : ViewModelBase, IFunctions
	{
		private ISettings settings;
		private IFunctions functions;

		public string K { get => functions.K; set => functions.K = value; }
		public string dK_du { get => functions.dK_du; set => functions.dK_du = value; }
		public string g { get => functions.g; set => functions.g = value; }
		public string InitCond { get => functions.InitCond; set => functions.InitCond = value; }
		public string LeftBoundCond { get => functions.LeftBoundCond; set => functions.LeftBoundCond = value; }
		public string RightBoundCond { get => functions.RightBoundCond; set => functions.RightBoundCond = value; }
		public string u { get => functions.u; set => functions.u = value; }
		public string du_dx { get => functions.du_dx; set => functions.du_dx = value; }
		public string d2u_dx2 { get => functions.d2u_dx2; set => functions.d2u_dx2 = value; }
		public string du_dt { get => functions.du_dt; set => functions.du_dt = value; }

		public FunctionsViewModel(Settings settings)
		{
			this.settings = settings;
			functions = settings.Functions;
		}

		private string CleanExpression(string expression)
		{
			return expression.Replace("Convert", "").Replace("Param_0.", "").Replace("(", "").Replace(")", "").Replace(" ", "");
		}
	}
}
