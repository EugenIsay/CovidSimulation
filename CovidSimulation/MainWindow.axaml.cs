using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
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
        List<double> Susceptible = new List<double>();
        List<double> Infected = new List<double>();
        List<double> Removed = new List<double>();
        List<double> Dead = new List<double>();

        public ISeries[] Series { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 100; i++)
            {
                humans.Add(new Human());
                humans[i].id = i;
                Ellipse ellipse = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Fill = Brush.Parse("PowderBlue"), Tag = i };
                if (humans[i].status == "Infected")
                    ellipse.Fill = Brush.Parse("Red");
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, humans[i].xCoordinate);
                Canvas.SetTop(ellipse, humans[i].yCoordinate);
            }
            MainChart.YAxes = new List<Axis>
            {
                new Axis
                {
                    Labels = Enumerable.Range(1, humans.Count).Select(s => s.ToString()).ToList(),
                }
            };
        }

        private async void ClickStart(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                humans[i].status = "Infected";
                Ellipse ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == humans[i].id) as Ellipse);
                ellipse.Fill = Brush.Parse("Red");
            }
            DispatcherTimer MoumentTimer;
            MoumentTimer = new DispatcherTimer();
            MoumentTimer.Interval = TimeSpan.FromMilliseconds(10);
            MoumentTimer.Tick += ChangePosition;
            MoumentTimer.Start();
            DispatcherTimer ChartTimer;
            ChartTimer = new DispatcherTimer();
            ChartTimer.Interval = TimeSpan.FromMilliseconds(150);
            ChartTimer.Tick += ChangeChart;
            ChartTimer.Start();
        }
        private void ChangePosition(object sender, EventArgs e)
        {
            foreach (var human in humans.Where(h => h.status != "Dead"))
            {
                human.Going();
                Ellipse ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id) as Ellipse);
                if (human.status == "Infected")
                {
                    ellipse.Fill = Brush.Parse("Tomato");
                    if (new Random().Next(1, 100) > 98)
                    {
                        human.status = "Dead";
                        ellipse.Fill = Brush.Parse("PaleVioletRed");
                    }
                }
                List<Human> stackedHumans = humans.Where(h => h.GetCoordinates == human.GetCoordinates).ToList();
                if (stackedHumans.Count() >= 2 && stackedHumans.Select(h => h.status).ToList().Contains("Infected"))
                {
                    foreach (var stakedHuman in stackedHumans.Where(h => h.status == "Susceptible"))
                    {
                        if (new Random().Next(1, 10) < 4)
                        {
                            humans[stakedHuman.id].status = "Infected";
                        }
                    }
                }
                Canvas.SetLeft(ellipse, human.xCoordinate);
                Canvas.SetTop(ellipse, human.yCoordinate);
            }

        }
        private void ChangeChart(object sender, EventArgs e)
        {
            Susceptible.Add(humans.Where(h => h.status == "Susceptible").Count());
            Infected.Add(humans.Where(h => h.status == "Infected").Count());
            Removed.Add(humans.Where(h => h.status == "Removed").Count());
            Dead.Add(humans.Where(h => h.status == "Dead").Count());
            Series = new ISeries[] {
                new StackedAreaSeries<double> { Values = Susceptible, Fill = new SolidColorPaint(SKColors.PowderBlue) },
                new StackedAreaSeries<double> { Values = Infected, Fill = new SolidColorPaint(SKColors.Tomato) },
                new StackedAreaSeries<double> { Values = Removed, Fill = new SolidColorPaint(SKColors.Gray) },
                new StackedAreaSeries<double> { Values = Dead, Fill = new SolidColorPaint(SKColors.PaleVioletRed) }
                };
            MainChart.Series = Series;
        }

    }
}