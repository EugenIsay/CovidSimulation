using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CovidSimulation
{
    public class Human
    {
        public string status { get; set; } = "Susceptible";
        public double xCoordinate { get; set; } = new Random().Next(0, 500);
        public double yCoordinate { get; set; } = new Random().Next(0, 500);

        public double xDestination { get; set; } = new Random().Next(0, 500);
        public double yDestination { get; set; } = new Random().Next(0, 500);

        public double speed { get; set; } = new Random().Next(1 , 10)/100;
        

        public bool ReachedDestination
        {
            get => xCoordinate == xDestination && yCoordinate == yDestination;
        }

        public Task Task()
        {
            if (!ReachedDestination)
            {

            }
            return System.Threading.Tasks.Task.CompletedTask;
        }



    }
}
