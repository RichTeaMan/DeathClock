using DeathClock.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PersistencePerson = DeathClock.Persistence.TmdbPerson;

namespace DeathClock
{
    public class PersonMapper
    {
        public PersistencePerson Map(Person person)
        {
            PersistencePerson persistencePerson = new PersistencePerson
            {
                BirthDate = person.BirthDate,
                DeathDate = person.DeathDate,
                IsDead = person.IsDead,
                Title = person.Title,
                ImdbUrl = person.Url,
                KnownFor = person.KnownFor
            };

            return persistencePerson;
        }

        public DeathClockData MapToDeathClockData(IEnumerable<Person> people)
        {
            var personArray = people.Select(p => Map(p)).ToArray();
            DeathClockData deathClockData = new DeathClockData
            {
                PersonList = personArray,
                CreatedOn = DateTimeOffset.Now
            };
            return deathClockData;
        }
    }
}
