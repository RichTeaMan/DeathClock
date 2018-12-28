using Newtonsoft.Json;

namespace DeathClock.Tmdb.Models
{
    public class PersonSearchResult
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("results")]
        public PersonSummary[] Results { get; set; }
    }
}
