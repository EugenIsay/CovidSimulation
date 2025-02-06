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
        Random random = new Random();
        DispatcherTimer CheckTimer;
        public Human()
        {
            xCoordinate = random.Next(1, 500);
            yCoordinate = random.Next(1, 500);
            ChangeDestination();
            speed = 1;

        }

        public int id;

        public string status { get; set; } = "Susceptible";
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
                if (!ReachedDestination && xCoordinate < 500 && xCoordinate > 0 && yCoordinate < 500 && yCoordinate > 0)
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
            xDestination = random.Next(1, 500);
            yDestination = random.Next(1, 500);
            double deltaX = xDestination - xCoordinate;
            double deltaY = yDestination - yCoordinate;
            vectorX = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            vectorY = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }

        public void InfectionStarted()
        {
            status = "Infected";
            CheckTimer = new DispatcherTimer();
            CheckTimer.Interval = TimeSpan.FromSeconds(1);
            CheckTimer.Tick += CheckHuman;
            CheckTimer.Start();
        }
        public void CheckHuman(object sender, EventArgs e)
        {
            sickDays++;
            if (sickDays > 14 && random.Next(1, 100) <= 12)
            {
                status = "Dead";
                CheckTimer.Stop();
            }
            if ((sickDays > 14 && random.Next(1, 100) <= 12) || (sickDays > 21 && random.Next(1, 100) <= 44) || sickDays == 38)
            {
                status = "Recovered";
                CheckTimer.Stop();
            }
        }

    }
}
