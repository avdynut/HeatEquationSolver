using HeatEquationSolver;
using System;
using System.Windows;

namespace HeatEquationSolverUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnswerTextBlock.Text = "";
                QuasiNewton qn = new QuasiNewton(double.Parse(a.Text), int.Parse(N.Text), double.Parse(T.Text),
                    int.Parse(M.Text) * 40, double.Parse(Beta0.Text), double.Parse(Eps.Text));
                AnswerTextBlock.Text = qn.Answer;
                Norm.Content = qn.Norm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
