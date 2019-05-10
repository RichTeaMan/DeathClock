using DeathClock.Persistence;
using DeathClock.Wikipedia.Models;
using RichTea.Common.Extensions;
using System;

using PersistencePerson = DeathClock.Persistence.WikipediaPerson;

namespace DeathClock
{
    public class WikipediaPersonMapper
    {
        private readonly Random random = new Random();

        public PersistencePerson Map(WikipediaJsonPerson person)
        {
            PersistencePerson persistencePerson = new PersistencePerson
            {
                BirthDate = person.BirthDate,
                DeathDate = person.DeathDate,
                IsDead = person.IsDead,
                Title = person.Title,
                Url = person.JsonUrl,
                IsStub = person.IsStub,
                WordCount = person.WordCount,
                DeathWordCount = person.DeathWordCount,
                KnownFor = string.Empty,
                RecordedDate = DateTime.Now,
                UpdateDate = CreatedUpdatedDate(),
                DataSet = person.IsStub ? Constants.WIKIPEDIA_STUB_NAME : Constants.WIKIPEDIA_DATASET_NAME
            };

            return persistencePerson;
        }

        private DateTime CreatedUpdatedDate()
        {
            var minimum = new TimeSpan(7, 0, 0, 0);
            var maximum = new TimeSpan(14, 0, 0, 0);

            var ticks = random.NextLong(minimum.Ticks, maximum.Ticks) + DateTime.Now.Ticks;

            var date = new DateTime(ticks);
            return date;
        }
    }
}
