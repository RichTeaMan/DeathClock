using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    public class Person
    {
        public static DateParser[] BirthDateParsers { get; private set; }
        public static DateParser[] DeathDateParsers { get; private set; }

        static Person()
        {
            BirthDateParsers = new DateParser[]
            { 
                new DateParser(@"(?i)(?<={{birth date and age\|(df=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d"),
                new DateParser(@"(?<=birth date\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))[^\|<]+", "d MMMM yyyy"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))[^\|<]+", "MMMM d, yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy"),
                new DateParser(@"(?<= born )[^\)]+", "d MMMM yyyy")
            };

            DeathDateParsers = new DateParser[]
            {
                new DateParser(@"(?i)(?<={{Death date and age\|(df=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<]+", "d MMMM yyyy"),
                new DateParser(@"(?<=death date\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<]+", "MMMM d, yyyy"),
                new DateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy"),
                new DateParser(@"(?<= died )[^\)]+", "d MMMM yyyy")
            };
        }

        public static string[] DeathWords = new string[] { "cancer", "ill", "sick", "accident" };
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
                var endDate = DeathDate ?? DateTime.Now;
                return endDate.Year - BirthDate.Year;
            }
        }
        public bool IsDead
        {
            get { return DeathDate != null; }
        }
        public int DeathWordCount { get; private set; }

        public static Person Create(string title)
        {
            var person = new Person();

            string jsonContent = Utilities.GetPage(title);
            Regex name = new Regex(@"(?<=name\s+=)[^\|]+");
            var nameMatch = name.Match(jsonContent);
            if (nameMatch.Success)
                person.Name = nameMatch.Value.Replace("\\n", "");
            else
                person.Name = title.Replace('_', ' ');

            DateTime? birth = null;
            foreach (var parser in BirthDateParsers)
            {
                birth = parser.GetDate(jsonContent);
                if (birth != null)
                    break;
            }
            if (birth == null)
                return null;
            person.BirthDate = birth.Value;

            DateTime? death = null;
            foreach (var parser in DeathDateParsers)
            {
                death = parser.GetDate(jsonContent);
                if (death != null)
                    break;
            }
            person.DeathDate = death;

            foreach (var word in DeathWords)
            {
                Regex wordRegex = new Regex(word);
                person.DeathWordCount += wordRegex.Matches(jsonContent).Count;
            }

            return person;

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
