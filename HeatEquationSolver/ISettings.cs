using HeatEquationSolver.BetaCalculators;
using HeatEquationSolver.Equations;

namespace HeatEquationSolver
{
	public interface ISettings
	{
		/// <summary>
		/// The first 'x' in the scope of the function
		/// </summary>
		double X1 { get; set; }

		/// <summary>
		/// The second 'x' in the scope of the function
		/// </summary>
		double X2 { get; set; }

		/// <summary>
		/// The first 't' in the scope of the function
		/// </summary>
		double T1 { get; set; }

		/// <summary>
		/// The second 't' in the scope of the function
		/// </summary>
		double T2 { get; set; }

		/// <summary>
		/// The number of segments in 'x'
		/// </summary>
		int N { get; set; }

		/// <summary>
		/// The number of segments in 't'
		/// </summary>
		int M { get; set; }

		/// <summary>
		/// Precision in calculating nonlinear systems
		/// </summary>
		double Epsilon { get; set; }

		/// <summary>
		/// Precision in calculating with different steps by 't' axis
		/// </summary>
		double Epsilon2 { get; set; }

		/// <summary>
		/// Multiplier in regularized method
		/// </summary>
		double Alpha { get; set; }

		/// <summary>
		/// The first multiplier 'beta'
		/// </summary>
		double Beta0 { get; set; }

		/// <summary>
		/// Method for calculating next beta on each step in iteration process
		/// </summary>
		BetaCalculator BetaCalculatorMethod { get; set; }

		/// <summary>
		/// Max number of iterations in calculating nonlinear systems
		/// If exceed, then throw Exception
		/// </summary>
		int MaxIterations { get; set; }

		/// <summary>
		/// If true, use parsed functions, else hard-coded functions
		/// </summary>
		bool UseParsedEquation { get; set; }

		/// <summary>
		/// Necessary strings for parsing functions
		/// </summary>
		Functions Functions { get; set; }
	}
}
