using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathClock.Persistence
{
    public class JsonPersistence
    {
        public async Task SavePersonsAsync(IEnumerable<Person> persons, string jsonFileLocation)
        {
            string json = JsonConvert.SerializeObject(persons.ToArray());
            await File.WriteAllTextAsync(jsonFileLocation, json);
        }

        public async Task<IEnumerable<Person>> LoadPersonsAsync(string jsonFileLocation)
        {
            string json = await File.ReadAllTextAsync(jsonFileLocation);
            var persons = JsonConvert.DeserializeObject<Person[]>(json);
            return persons;
        }
    }
}
