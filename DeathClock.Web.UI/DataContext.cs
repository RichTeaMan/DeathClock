
using DeathClock.Persistence;

namespace DeathClock.Web.UI
{
    public class DataContext
    {
        public DeathClockData[] DeathClockDataSet { get; set; }

        public TmdbPerson[] Persons { get; set; }

    }
}
