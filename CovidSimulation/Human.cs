using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CovidSimulation
{
    public class Human
    {
        int xMax = 501;
        int yMax = 501;
        Random random = new Random();
        DispatcherTimer CheckTimer;
        public Human()
        {
            xCoordinate = random.Next(1, 499);
            yCoordinate = random.Next(1, 499);
            ChangeDestination();
            speed = 1;

        }

        public int id;

        public string status { get; set; } = "Susceptible";
        public bool knowAboutInfecion = false;
        public bool inQuarantine = false;
        int daysInQuarantine = 0;
        public int sickDays { get; set; }

        public double xCoordinate { get; set; }
        public double yCoordinate { get; set; }
        public (int, int) GetCoordinates
        {
            get => ((int)xCoordinate, (int)yCoordinate);
        }

        public double xDestination { get; set; }
        public double yDestination { get; set; }

        public double speed { get; set; }

        public double vectorX { get; set; }
        public double vectorY { get; set; }


        public bool ReachedDestination
        {
            get
            {
                double deltaX = xDestination - xCoordinate;
                double deltaY = yDestination - yCoordinate;
                double vectorXNew = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                double vectorYNew = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                return (vectorXNew > 0) != (vectorX > 0) && (vectorYNew > 0) != (vectorY > 0);
            }
        }

        public void Going()
        {
            if (status != "Dead")
                if (!ReachedDestination && xCoordinate < xMax && xCoordinate > 0 && yCoordinate < yMax && yCoordinate > 0)
                {
                    xCoordinate = xCoordinate + speed * vectorX;
                    yCoordinate = yCoordinate + speed * vectorY;
                }
                else
                {
                    ChangeDestination();
                }
        }

        public void ChangeDestination()
        {
            xDestination = random.Next(1, xMax);
            yDestination = random.Next(1, yMax);
            double deltaX = xDestination - xCoordinate;
            double deltaY = yDestination - yCoordinate;
            vectorX = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            vectorY = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }

        public void InfectionStarted()
        {
            status = "Infected";
            knowAboutInfecion = false;
            CheckTimer = new DispatcherTimer();
            CheckTimer.Interval = TimeSpan.FromSeconds(1);
            CheckTimer.Tick += CheckHuman;
            CheckTimer.Start();
        }

        public void QuarantineStarted()
        {
            xMax = 129;
            yMax = 129;
            xCoordinate = 64;
           yCoordinate = 64;
            inQuarantine = true;
            ChangeDestination();
        }

        public void CheckHuman(object sender, EventArgs e)
        {
            sickDays++;
            if (sickDays > 14 && random.Next(1, 100) <= Stats.DeathChance)
            {
                status = "Dead";
                CheckTimer.Stop();
            }
            if ((sickDays > 14 && random.Next(1, 100) <= 12) || (sickDays > 21 && random.Next(1, 100) <= 44) || sickDays == 38)
            {
                status = "Recovered";
                CheckTimer.Stop();
            }
            if (!knowAboutInfecion && random.Next(1, 100) >= Stats.UndetectedChance )
            {
                knowAboutInfecion = true;
            }
            if (inQuarantine)
            {
                daysInQuarantine++;
                if (daysInQuarantine >= Stats.IsolationPeriod)
                {
                    inQuarantine = false;
                    xCoordinate = 250;
                    yCoordinate = 250;
                    xMax = 501;
                    yMax = 501;
                    knowAboutInfecion = false; 
                    ChangeDestination();
                }
            }
        }

    }
}
