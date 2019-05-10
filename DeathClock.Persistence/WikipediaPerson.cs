using System;
using System.ComponentModel.DataAnnotations;

namespace DeathClock.Persistence
{
    /// <summary>
    /// Contains Wikipedia person details.
    /// </summary>
    public class WikipediaPerson : BasePerson
    {
        [Required]
        public int WordCount { get; set; }

        [Required]
        public int DeathWordCount { get; set; }

        [Required]
        public bool IsStub { get; set; }
    }
}
