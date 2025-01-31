using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

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
            Ellipse ellipse = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Fill= Brush.Parse("Green") };
            canvas.Children.Add(ellipse);
            Ellipse ellipse2 = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Fill = Brush.Parse("Red") };
            canvas.Children.Add(ellipse2);
        }

        private void ClickStart(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Task.WaitAll();
        }
    }
}