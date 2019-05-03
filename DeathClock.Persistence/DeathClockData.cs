using System;
using System.Collections.Generic;
using System.Text;

namespace DeathClock.Persistence
{
    [Obsolete]
    public class DeathClockData
    {
        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public TmdbPerson[] PersonList { get; set; }
    }
}
