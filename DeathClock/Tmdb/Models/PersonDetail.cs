using Newtonsoft.Json;
using System;

namespace DeathClock.Tmdb.Models
{
    public class PersonDetail
    {
        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }

        [JsonProperty("known_for_department")]
        public string KnownForDepartment { get; set; }

        [JsonProperty("deathday")]
        public DateTime? DeathDay { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("also_known_as")]
        public string[] AlsoKnownAs { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("popularity")]
        public float Popularity { get; set; }

        [JsonProperty("place_of_birth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("homepage")]
        public string HomePage { get; set; }
    }

}
