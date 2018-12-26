using System;

namespace DeathClock
{
    /// <summary>
    /// Contains person details.
    /// </summary>
    public class Person
    {
        
        public string Name { get; private set; }
        public DateTime BirthDate { get; private set; }
        public DateTime? DeathDate { get; private set; }

        /// <summary>
        /// Age in years.
        /// </summary>
        public int Age
        {
            get
            {
                try
                {
                    var endDate = DeathDate ?? DateTime.Now;
                    return (DateTime.MinValue + (endDate - BirthDate)).Year - 1;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public bool IsDead
        {
            get { return DeathDate != null || Age >= 120; }
        }
        public int DeathWordCount { get; private set; }

        public string Title { get; private set; }

        public int WordCount { get; private set; }

        public string KnownFor { get; private set; }

        public bool IsStub
        {
            get { return WordCount < 2000; }
        }

        public string Url
        {
            get
            {
                return string.Format(WikiUtility.Url, Title.Replace(' ', '_'));
            }
        }

        public string JsonUrl
        {
            get
            {
                return string.Format(WikiUtility.apiUrl, Title);
            }
        }

        public Person(string name, DateTime birthDate, DateTime? deathDate, int deathWordCount, string title, int wordCount, string knownFor)
        {
            Name = name;
            BirthDate = birthDate;
            DeathDate = deathDate;
            DeathWordCount = deathWordCount;
            Title = title;
            WordCount = wordCount;
            KnownFor = knownFor;
        }

        public override string ToString()
        {
            return string.Format("Name: {0} Age: {1} Birth: {2} Is Dead: {3} Death word count: {4}",
                Name,
                Age,
                BirthDate.ToShortDateString(),
                IsDead,
                DeathWordCount);
        }
    }
}
