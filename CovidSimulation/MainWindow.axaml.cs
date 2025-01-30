using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CovidSimulation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<double> a = new List<double>() { 0, 6, 5, 7, 8, 9 };
            ISeries[] series = new ISeries[] { new StackedAreaSeries<double> { Values = a} };
            MainChart.Series = series;
        }
    }
}