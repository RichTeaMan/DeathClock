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
        public async Task SaveDeathClockDataAsync(DeathClockData deathClockData, string jsonFileLocation)
        {
            string json = JsonConvert.SerializeObject(deathClockData, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFileLocation, json);
        }

        public async Task<DeathClockData> LoadDeathClockDataAsync(string jsonFileLocation)
        {
            string json = await File.ReadAllTextAsync(jsonFileLocation);
            var deathClockData = JsonConvert.DeserializeObject<DeathClockData>(json);
            return deathClockData;
        }
    }
}
