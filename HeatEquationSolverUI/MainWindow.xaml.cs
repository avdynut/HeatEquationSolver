﻿using HeatEquationSolver;
using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace HeatEquationSolverUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Task task;
        CancellationTokenSource source = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnswerTextBlock.Text = "";
                var qn = new Solver();
                

                var progressIndicator = new Progress<int>(ReportProgress);
                task = Task.Run(() => qn.Solve(source.Token, progressIndicator));
                await task;
                
                AnswerTextBlock.Text = qn.Answer; //actual.Solution.Aggregate("", (current, xi) => current + (xi + "\n")).TrimEnd();
                Norm.Content = qn.Norm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //StartButton.IsEnabled = RecountButton.IsEnabled = true;
            //StopButton.IsEnabled = false;
        }

        void ReportProgress(int value)
        {
            //NumberOfIterations.Content = value;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            source.Cancel();
        }
    }
}
