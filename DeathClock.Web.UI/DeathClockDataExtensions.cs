using DeathClock.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeathClock.Web.UI
{
    public static class DeathClockDataExtensions
    {
        public static IEnumerable<BasePerson> MostRisk(this IEnumerable<BasePerson> personList)
        {
            var orderedPersons = personList
                .Where(p => !p.IsDead)
                .OrderByDescending(p => p.Age)
                .ToArray();
            return orderedPersons;
        }

        public static IEnumerable<BasePerson> ByDeathYear(this IEnumerable<BasePerson> personList, int deathYear)
        {
            var orderedPersons = personList
                .Where(p => p.DeathDate?.Year == deathYear)
                .OrderBy(p => p.DeathDate)
                .ToArray();
            return orderedPersons;
        }
    }
}
