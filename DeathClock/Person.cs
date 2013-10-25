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

        public Person(string title)
        {
            string jsonContent = Utilities.GetPage(title);
            Regex name = new Regex(@"(?<=name\s+=)[^\n\|]+");
            var nameMatch = name.Match(jsonContent);
            if (nameMatch.Success)
                Name = nameMatch.Value.Trim().Replace("\n", "");
            else
                Name = title.Replace('_', ' ');

            DateTime? birth;
            birth = GetDate(jsonContent,
                @"(?i)(?<={{birth date and age\|(df=y(es|)\||))\d+\|\d+\|\d+",
                "yyyy|M|d");

            if (birth == null)
            {
                birth = GetDate(jsonContent, @"(?<=birth_date\s+=\s+)[^\|]+", "d MMMM yyyy");
            }
            if (birth == null)
                throw new ArgumentException("A birthdate could not be found");

            BirthDate = birth.Value;

            DateTime? death;
            death = GetDate(jsonContent,
                @"(?i)(?<={{Death date and age\|(df=y(es|)\||))\d+\|\d+\|\d+",
                "yyyy|M|d");
            if (death == null)
            {
                death = GetDate(jsonContent, @"(?<=death_date\s+=\s+)[^\|]+", "d MMMM yyyy");
            }

            DeathDate = death;

            foreach (var word in DeathWords)
            {
                Regex wordRegex = new Regex(word);
                DeathWordCount += wordRegex.Matches(jsonContent).Count;
            }

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

        private DateTime? GetDate(string content, string regex, string dateFormat)
        {
            Regex dateRegex = new Regex(regex);

            var match = dateRegex.Match(content);
            if (match.Success)
            {
                try
                {
                    var date = DateTime.ParseExact(match.Value, dateFormat, null);
                    return date;
                }
                catch { }

            }
            return null;
        }
    }
}
