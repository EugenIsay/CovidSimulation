using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovidSimulation
{
    public static class Stats
    {
        // Вероятность заражения (1-100)
        public static int InfectionChance { get; set; }
        // Вероятность соблюдения изоляции (0-100)
        public static int IsolationChance { get; set; }
        // Длительность изоляции (0-бесконечность)
        public static int IsolationPeriod { get; set; }
        // Доля невыявленных (0-100)
        public static int UndetectedChance { get; set; }

        // Смертность
        public static int DeathChance { get; set ; }

    }
}
