using DeathClock.Persistence;
using System.Linq;

namespace DeathClock.Web.UI
{
    public class DataContext
    {
        public DeathClockData DeathClockData { get; set; }

        public DeathClockData MostRisk()
        {
            var orderedPersons = DeathClockData.PersonList
                .Where(p => !p.IsDead && !p.IsStub)
                .OrderByDescending(p => p.Age)
                .Take(100)
                .ToArray();
            var result = new DeathClockData
            {
                PersonList = orderedPersons
            };
            return result;
        }
    }
}
