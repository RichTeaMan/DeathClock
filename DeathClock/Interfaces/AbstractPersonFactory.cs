using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeathClock.Persistence;
using Microsoft.Extensions.Logging;
using RichTea.Common.Extensions;

namespace DeathClock
{
    public abstract class AbstractPersonFactory<TPerson>: IPersonFactory<TPerson> where TPerson : BasePerson
    {
        private readonly ILogger<AbstractPersonFactory<TPerson>> logger;

        protected readonly Random random = new Random();

        public abstract Task FindNewPersons();


        public AbstractPersonFactory(ILogger<AbstractPersonFactory<TPerson>> logger)
        {
            this.logger = logger;
        }

        public async Task UpdateExistingPersons()
        {
            logger.LogDebug("Updating existing persons.");
            var existingPersons = await FetchExistingPersons();
            var personsToUpdate = new List<TPerson>();

            foreach(var existingPerson in existingPersons)
            {
                try
                {
                    var personShouldUpdate = await UpdatePerson(existingPerson);
                    if (personShouldUpdate)
                    {
                        personsToUpdate.Add(existingPerson);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Could not update person: {ex.Message}");
                }
            }

            logger.LogDebug($"Updating {personsToUpdate.Count} persons.");
            await StoreUpdatedPersons(personsToUpdate);
        }

        protected abstract Task<IEnumerable<TPerson>> FetchExistingPersons();

        /// <summary>
        /// Updates the given TPerson. The person may later be saved.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        protected abstract Task<bool> UpdatePerson(TPerson person);

        protected abstract Task StoreUpdatedPersons(IEnumerable<TPerson> personsToUpdate);

        protected DateTime CreatedUpdatedDate()
        {
            var minimum = new TimeSpan(7, 0, 0, 0);
            var maximum = new TimeSpan(14, 0, 0, 0);

            var ticks = random.NextLong(minimum.Ticks, maximum.Ticks) + DateTime.Now.Ticks;

            var date = new DateTime(ticks);
            return date;
        }

    }
}