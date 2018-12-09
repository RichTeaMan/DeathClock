using System;

namespace DeathClock.Persistence
{
    /// <summary>
    /// Contains person details.
    /// </summary>
    public class Person
    {

        public DateTime BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public int Age { get; set; }

        public bool IsDead { get; set; }

        public int DeathWordCount { get; set; }

        public string Title { get; set; }

        public int WordCount { get; set; }

        public string Description { get; set; }

        public bool IsStub { get; set; }

        public string Url { get; set; }
    }
}
