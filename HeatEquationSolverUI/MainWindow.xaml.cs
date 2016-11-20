using HeatEquationSolver;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HeatEquationSolverUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource source;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SolveButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            SolveButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            source = new CancellationTokenSource();
            try
            {
                Norm.Content = AnswerTextBlock.Text = "";
                var qn = new Solver();                

                var progressIndicator = new Progress<int>(ReportProgress);
                var task = Task.Run(() => qn.Solve(source.Token, progressIndicator));
                await task;
                
                AnswerTextBlock.Text = qn.Answer; //actual.Solution.Aggregate("", (current, xi) => current + (xi + "\n")).TrimEnd();
                Norm.Content = qn.Norm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SolveButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        void ReportProgress(int value)
        {
            ProgressBar.Value = value + 1;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            source.Cancel();
        }
    }
}
