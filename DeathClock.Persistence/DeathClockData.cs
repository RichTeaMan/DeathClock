using System;
using System.Collections.Generic;
using System.Text;

namespace DeathClock.Persistence
{
    public class DeathClockData
    {
        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public Person[] PersonList { get; set; }
    }
}
