using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Timers;

namespace CovidSimulation
{
    public partial class MainWindow : Window
    {
        List<Human> humans = new List<Human>();

        public MainWindow()
        {
            InitializeComponent();
            List<double> a = new List<double>() { 0, 6, 5, 7, 8, 9 };
            ISeries[] series = new ISeries[] { new StackedAreaSeries<double> { Values = a } };
            MainChart.Series = series;
            for (int i = 0; i < 1; i++)
            {
                humans.Add(new Human());
                humans[i].id = i;
                Ellipse ellipse = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Fill = Brush.Parse("Green"), Tag = i };
                if (humans[i].status == "Infected")
                    ellipse.Fill = Brush.Parse("Red");
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, humans[i].xCoordinate);
                Canvas.SetTop(ellipse, humans[i].yCoordinate);
            }
            humans[0].status = "Infected";
        }

        private void ClickStart(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //Timer timer = new Timer(1000);
            //timer.Elapsed += async (sender, e) => await ChangePosition();
            //timer.Start();
        }
        private Task ChangePosition()
        {
            foreach (var human in humans)
            {
                human.Going();
                Ellipse ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id) as Ellipse);
                Canvas.SetLeft(ellipse, human.xCoordinate);
                Canvas.SetTop(ellipse, human.yCoordinate);
            }
            return Task.CompletedTask;
        }
    }
}