using Newtonsoft.Json;

namespace DeathClock.Tmdb.Models
{
    public class PersonSummary
    {
        [JsonProperty("popularity")]
        public float Popularity { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("known_for")]
        public KnownFor[] KnownFor { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }
    }
}
