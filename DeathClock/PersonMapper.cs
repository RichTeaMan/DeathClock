using System;
using System.Collections.Generic;
using System.Text;

using PersistencePerson = DeathClock.Persistence.Person;

namespace DeathClock
{
    public class PersonMapper
    {
        public PersistencePerson Map(Person person)
        {
            PersistencePerson persistencePerson = new PersistencePerson
            {
                Age = person.Age,
                BirthDate = person.BirthDate,
                DeathDate = person.DeathDate,
                DeathWordCount = person.DeathWordCount,
                Description = person.Description,
                IsDead = person.IsDead,
                IsStub = person.IsStub,
                Title = person.Title,
                Url = person.Url,
                WordCount = person.WordCount
            };

            return persistencePerson;
        }
    }
}
