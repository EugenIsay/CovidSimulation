using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidSimulation
{
    public class Human
    {
        // status определяет группу человека, где 1 => уязвимые,
        // 2 => инфицированные, 3 => выздоровевшие и 4 => умершие.
        public int status { get; set; } = 1;
        public int xCoordinate { get; set; }
        public int yCoordinate { get; set; }
    }
}
