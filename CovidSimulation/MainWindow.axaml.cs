using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Serialization;
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
        List<int> Days = new List<int>();

        DispatcherTimer ChartTimer;
        DispatcherTimer MoumentTimer;
        DispatcherTimer DaysTimer;



        int CurrentDay = 1;

        public ISeries[] Series { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AddHumans();
        }

        private async void ClickStart(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            if (humans.Count == 0)
                AddHumans();
            Susceptible.Clear();
            Infected.Clear();
            Recovered.Clear();
            Dead.Clear();
            StatsPanel.IsEnabled = false;
            Stats.InfectionChance = (int)CP.Value;
            Stats.InfectionChance = (int)SD.Value;
            Stats.IsolationPeriod = (int)IP.Value;
            Stats.UndetectedChance = (int)AC.Value;
            Stats.DeathChance = (int)DC.Value;
            MoumentTimer = new DispatcherTimer();
            MoumentTimer.Interval = TimeSpan.FromMilliseconds(10);
            MoumentTimer.Tick += ChangePosition;
            MoumentTimer.Start();
            ChartTimer = new DispatcherTimer();
            ChartTimer.Interval = TimeSpan.FromMilliseconds(125);
            ChartTimer.Tick += ChangeChart;
            ChartTimer.Start();

            //DaysTimer = new DispatcherTimer();
            //DaysTimer.Interval = TimeSpan.FromSeconds(1);
            //DaysTimer.Tick += DayPassed;
            //DaysTimer.Start();
        }
        private void ChangePosition(object sender, EventArgs e)
        {
            foreach (var human in humans)
            {
                Ellipse ellipse = new Ellipse();
                if (!human.inQuarantine)
                {
                    ellipse = (canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id) as Ellipse);
                    if (ellipse == null)
                    {
                        ellipse = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Tag = human.id };
                        canvas.Children.Add(ellipse);
                        Canvas.SetLeft(ellipse, human.xCoordinate);
                        Canvas.SetTop(ellipse, human.yCoordinate);
                        if (QuarantineCanvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id) != null)
                        {
                            QuarantineCanvas.Children.Remove(QuarantineCanvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id));
                        }
                    }
                }
                else
                {
                    ellipse = (QuarantineCanvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id) as Ellipse);
                    if (ellipse == null)
                    {
                        ellipse = new Ellipse() { Width = 5, Height = 5, Opacity = 1, Tag = human.id };
                        QuarantineCanvas.Children.Add(ellipse);
                        Canvas.SetLeft(ellipse, human.xCoordinate);
                        Canvas.SetTop(ellipse, human.yCoordinate);
                    }
                }
                human.Going();
                switch (human.status)
                {
                    case "Infected":
                        if (human.knowAboutInfecion)
                        {
                            ellipse.Fill = Brush.Parse("Tomato"); break;
                        }
                        else
                        {
                            ellipse.Fill = Brush.Parse("Yellow"); break;
                        }
                    case "Dead":
                        ellipse.Fill = Brush.Parse("PaleVioletRed");
                        break;
                    case "Recovered":
                        ellipse.Fill = Brush.Parse("Gray");
                        break;
                }
                List<Human> stackedHumans = humans.Where(h => Math.Sqrt(Math.Pow(h.xCoordinate - human.xCoordinate, 2) + Math.Pow(h.yCoordinate - human.yCoordinate, 2)) < 3 && !h.inQuarantine).ToList();

                if (stackedHumans.Count() >= 2 && stackedHumans.Select(h => h.status).ToList().Contains("Infected"))
                {
                    foreach (var stakedHuman in stackedHumans.Where(h => h.status == "Susceptible"))
                    {
                        if (random.Next(1, 100) < Stats.InfectionChance)
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

            if (humans.Where(h => h.status == "Infected" && h.knowAboutInfecion).Count() > 50)
            {
                List<Human> toQuarantine = humans.Where(h => h.status == "Infected" && !h.inQuarantine && h.knowAboutInfecion).ToList();
                foreach (Human human in toQuarantine)
                {
                    canvas.Children.Remove(canvas.Children.FirstOrDefault(e => int.Parse(e.Tag.ToString()) == human.id));
                    humans[human.id].QuarantineStarted();
                }    
            }

            //MainChart.XAxes = new List<Axis>
            //{
            //    new Axis()
            //    {
            //        Labels = Days.Select(d => d.ToString()).ToList(),
            //    }
            //};

            Removed.Text = $"# Выбывшие {Recovered.Last() + Dead.Last()}";
            Active.Text = $"# Активные случаи {Infected.Last()}";
            if (humans.Where(h => h.status == "Infected").Count() == 0)
            {
                StopTimers();
            }
        }
        private void DayPassed(object sender, EventArgs e)
        {
            CurrentDay++;
            Days.Add(CurrentDay);
        }
        public void StopTimers()
        {
            ChartTimer.Stop();
            MoumentTimer.Stop();
            humans.Clear();
            Start.IsEnabled = true;
            StatsPanel.IsEnabled = true;
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
                    ellipse.Fill = Brush.Parse("Yellow");
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