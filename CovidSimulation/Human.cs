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
        public Human()
        {
            xCoordinate = random.Next(1, 500);
            yCoordinate = random.Next(1, 500);
            xDestination = random.Next(1, 500);
            yDestination = random.Next(1, 500);
            double deltaX = xDestination - xCoordinate;
            double deltaY = yDestination - yCoordinate;
            vectorX = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            vectorY = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            speed = 1;

        }

        public int id;

        public string status { get; set; } = "Susceptible";

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
            if (status!= "Dead")
            if (!ReachedDestination && xCoordinate < 500 && xCoordinate > 0 && yCoordinate < 500 && yCoordinate > 0)
            {
                xCoordinate = xCoordinate + speed * vectorX;
                yCoordinate = yCoordinate + speed * vectorY;
            }
            else
            {
                xDestination = random.Next(1, 500);
                yDestination = random.Next(1, 500);
                double deltaX = xDestination - xCoordinate;
                double deltaY = yDestination - yCoordinate;
                vectorX = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                vectorY = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            }
        }

    }
}
