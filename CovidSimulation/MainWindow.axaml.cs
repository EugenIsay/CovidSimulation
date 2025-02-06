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
        Random random = new Random();
        List<Human> humans = new List<Human>();
        List<double> Susceptible = new List<double>();
        List<double> Infected = new List<double>();
        List<double> Recovered = new List<double>();
        List<double> Dead = new List<double>();

        DispatcherTimer ChartTimer;
        DispatcherTimer MoumentTimer;

        public ISeries[] Series { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AddHumans();

        }

        private async void ClickStart(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (humans.Count == 0)
                AddHumans();
            Susceptible.Clear();
            Infected.Clear();
            Recovered.Clear();
            Dead.Clear();
            MoumentTimer = new DispatcherTimer();
            MoumentTimer.Interval = TimeSpan.FromMilliseconds(10);
            MoumentTimer.Tick += ChangePosition;
            MoumentTimer.Start();
            ChartTimer = new DispatcherTimer();
            ChartTimer.Interval = TimeSpan.FromMilliseconds(150);
            ChartTimer.Tick += ChangeChart;
            ChartTimer.Start();

        }
        private void ChangePosition(object sender, EventArgs e)
        {
            foreach (var human in humans)
            {
                human.Going();
                Ellipse ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id) as Ellipse);
                switch (human.status)
                {
                    case "Infected":
                        ellipse.Fill = Brush.Parse("Tomato"); break;
                    case "Dead":
                        ellipse.Fill = Brush.Parse("PaleVioletRed");
                        break;
                    case "Recovered":
                        ellipse.Fill = Brush.Parse("Gray");
                        break;
                }
                List<Human> stackedHumans = humans.Where(h => Math.Sqrt(Math.Pow(h.xCoordinate - human.xCoordinate, 2) + Math.Pow(h.yCoordinate - human.yCoordinate, 2)) < 5).ToList();

                if (stackedHumans.Count() >= 2 && stackedHumans.Select(h => h.status).ToList().Contains("Infected"))
                {
                    foreach (var stakedHuman in stackedHumans.Where(h => h.status == "Susceptible"))
                    {
                        if (random.Next(1, 10) < 3)
                        {
                            humans[stakedHuman.id].InfectionStarted();
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
            Recovered.Add(humans.Where(h => h.status == "Recovered").Count());
            Dead.Add(humans.Where(h => h.status == "Dead").Count());
            Series = new ISeries[] {
                new StackedAreaSeries<double> { Values = Infected, Fill = new SolidColorPaint(SKColors.Tomato), Name="Инфицированные" },
                new StackedAreaSeries<double> { Values = Susceptible, Fill = new SolidColorPaint(SKColors.PowderBlue), Name="Уязвимые" },
                new StackedAreaSeries<double> { Values = Recovered, Fill = new SolidColorPaint(SKColors.Gray), Name="Выздоровевшие" },
                new StackedAreaSeries<double> { Values = Dead, Fill = new SolidColorPaint(SKColors.PaleVioletRed), Name="Умершие" }
                };
            MainChart.Series = Series;
            Removed.Text = $"# Выбывшие {Recovered.Last() + Dead.Last()}" ;
            Active.Text = $"# Активные случаи {Infected.Last()}";
            if (humans.Where(h => h.status == "Infected").Count() == 0)
            {
                StopTimers();
            }
        }
        public void StopTimers()
        {
            ChartTimer.Stop();
            MoumentTimer.Stop();
            humans.Clear();
        }
        public void AddHumans()
        {
            canvas.Children.Clear();
            for (int i = 0; i < 500; i++)
            {
                humans.Add(new Human());
                humans[i].id = i;
                Ellipse ellipse = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Fill = Brush.Parse("PowderBlue"), Tag = i };
                if (humans[i].status == "Infected")
                    ellipse.Fill = Brush.Parse("Red");
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, humans[i].xCoordinate);
                Canvas.SetTop(ellipse, humans[i].yCoordinate);
                MainChart.LegendTextPaint = new SolidColorPaint(SKColors.White);
            }
            for (int i = 0; i < 5; i++)
            {
                humans[i].InfectionStarted();
                Ellipse ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == humans[i].id) as Ellipse);
                ellipse.Fill = Brush.Parse("Red");
            }
        }


    }
}