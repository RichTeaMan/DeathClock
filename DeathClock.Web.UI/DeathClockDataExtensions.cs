using DeathClock.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeathClock.Web.UI
{
    public static class DeathClockDataExtensions
    {
        public static Person[] MostRisk(this DeathClockData deathClockData)
        {
            var orderedPersons = deathClockData.PersonList
                .Where(p => !p.IsDead)
                .OrderByDescending(p => p.Age)
                .Take(100)
                .ToArray();
            return orderedPersons;
        }

        public static Person[] ByDeathYear(this DeathClockData deathClockData, int deathYear)
        {
            var orderedPersons = deathClockData.PersonList
                .Where(p => p.DeathDate?.Year == deathYear)
                .OrderBy(p => p.DeathDate)
                .ToArray();
            return orderedPersons;
        }
    }
}
