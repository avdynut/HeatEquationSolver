﻿using HeatEquationSolver;
using System;
using System.Linq;
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
            if (SolveButton.Content.ToString() == "Отмена")
            {
                SolveButton.IsEnabled = false;
                source.Cancel();
                return;
            }

            source = new CancellationTokenSource();
            try
            {
                Norm.Content = AnswerTextBlock.Text = "";
                var qn = new Solver();

                var progressIndicator = new Progress<int>(ReportProgress);
                var task = Task.Run(() => qn.Solve(source.Token, progressIndicator));
                SolveButton.Content = "Отмена";
                await task;

                AnswerTextBlock.Text = qn.Answer.Aggregate("", (current, xi) => current + (xi + "\n")).TrimEnd();
                Norm.Content = qn.Norm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SolveButton.Content = "Решить";
            SolveButton.IsEnabled = true;
        }

        void ReportProgress(int value)
        {
            ProgressBar.Value = ++value;
        }
    }
}
