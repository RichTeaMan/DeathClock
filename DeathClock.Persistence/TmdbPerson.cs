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
        public int TmdbId { get; set; }

        [Required]
        public double Popularity { get; set; }
    }
}
