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
                .Where(p => !p.IsDead && !p.IsStub)
                .OrderByDescending(p => p.Age)
                .Take(100)
                .ToArray();
            return orderedPersons;
        }
    }
}
