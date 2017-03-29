using System;

namespace HeatEquationSolver.Helpers
{
	public static class Matrix
	{
		public static double[,] Multiply(this double[,] a, double[,] b)
		{
			int t = a.GetLength(1);
			if (t != b.GetLength(0))
				throw new ArgumentException("Matrixes must be nxm and mxp");

			int n = a.GetLength(0);
			int m = b.GetLength(1);
			double[,] c = new double[n, m];

			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					for (int k = 0; k < t; k++)
						c[i, j] += a[i, k] * b[k, j];
			return c;
		}

		public static double[] Multiply(this double[,] a, double[] vector)
		{
			int n = vector.Length;
			if (n != a.GetLength(1))
				throw new ArgumentException("Matrix must be mxn and vector nx1");

			int m = a.GetLength(0);
			double[] result = new double[m];

			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					result[i] += a[i, j] * vector[j];
			return result;
		}

		public static double[,] AddDiag(this double[,] matrix, double diagElement)
		{
			double[,] a = (double[,])matrix.Clone();
			for (int i = 0; i < a.GetLength(0); i++)
				a[i, i] += diagElement;
			return a;
		}

		public static double[,] Transpose(this double[,] a)
		{
			int n = a.GetLength(1);
			int m = a.GetLength(0);
			double[,] t = new double[n, m];

			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					t[i, j] = a[j, i];

			return t;
		}
	}
}
