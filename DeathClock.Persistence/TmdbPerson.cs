using System;
using System.ComponentModel.DataAnnotations;

namespace DeathClock.Persistence
{
    /// <summary>
    /// Contains TMDB person details.
    /// </summary>
    public class TmdbPerson : BasePerson
    {
        [Required]
        [MaxLength(Constants.MAX_URL_LENGTH)]
        public string ImdbUrl { get; set; }

        [Required]
        public int TmdbId { get; set; }

        [Required]
        public string KnownFor { get; set; }

        [Required]
        public double Popularity { get; set; }
    }
}
