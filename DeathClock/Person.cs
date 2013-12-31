using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DeathClock
{
    public class Person
    {
        public static DateParser[] BirthDateParsers { get; private set; }
        public static DateParser[] DeathDateParsers { get; private set; }

        private static ConcurrentStack<string> errors;

        static Person()
        {
            errors = new ConcurrentStack<string>();
            BirthDateParsers = new DateParser[]
            { 
                new DateParser(@"(?i)(?<={{birth[ -_]date and age\|(df=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d"),
                new DateParser(@"(?<=birth date(\s+|)\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))[^\|<\(]+", "d MMMM yyyy"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))[^\|<\(]+", "MMMM d yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy"),
                new DateParser(@"(?<= born )[^\)]+", "d MMMM yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM d yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+", "yyyy"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))\d+", "yyyy"),
                new DateParser(@"(?i)(?<={{birth-date\|)\d+", "yyyy"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy"),
                new DateParser(@"(?<=\(c\. )\d+", "yyyy"),
            };

            DeathDateParsers = new DateParser[]
            {
                new DateParser(@"(?i)(?<={{Death date and age\|(df=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<\(]+", "d MMMM yyyy"),
                new DateParser(@"(?<=Date of death\|)[^\]]+", "d MMMM yyyy"),
                new DateParser(@"(?<=death-date and age\|df=yes\|)[^\|\]]+", "d MMMM yyyy"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<]+", "d MMMM yyyy"),
                new DateParser(@"(?<=death date\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<]+", "MMMM d yyyy"),
                new DateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy"),
                new DateParser(@"(?<= died )[^\)]+", "d MMMM yyyy"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<\(]+", "yyyy"),
                new DateParser(@"(?<=death year and age\|df=yes\|)[^\|\]]+", "yyyy"),
                new DateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\d+", "yyyy"),
                new DateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy"),
                new DateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM d yyyy"),
                new DateParser(@"(?<=Death date and age\|mf=yes\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM dd yyyy"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|){{dda\|)\d+\|\d+\|\d+", "yyyy|MM|dd")
            };
        }

        public static string[] DeathWords = new string[] { "cancer", "ill", "sick", "accident", "heart attack", "stroke" };
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
            get { return DeathDate != null || Age >= 110; }
        }
        public int DeathWordCount { get; private set; }

        public string Title { get; private set; }

        public int WordCount { get; private set; }

        public string Url
        {
            get
            {
                return string.Format(Utilities.Url, Title.Replace(' ', '_'));
            }
        }

        public string JsonUrl
        {
            get
            {
                return string.Format(Utilities.apiUrl, Title);
            }
        }

        public static Person Create(string title)
        {
            var person = new Person();

            string jsonContent = Utilities.GetPage(title);

            person.Title = GetTitle(jsonContent);

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
            

            DateTime? death = null;
            foreach (var parser in DeathDateParsers)
            {
                death = parser.GetDate(jsonContent);
                if (death != null)
                    break;
            }

            if (birth == null)
            {
                if (death != null)
                    LogError("{0} has a death date but no birth date.", person.Title);
                return null;
            }
            person.BirthDate = birth.Value;
            person.DeathDate = death;

            foreach (var word in DeathWords)
            {
                Regex wordRegex = new Regex(word);
                person.DeathWordCount += wordRegex.Matches(jsonContent).Count;
            }

            person.WordCount = jsonContent.Count(c => c == ' ');

            return person;

        }

        private static void LogError(string message, params object[] args)
        {
            errors.Push(string.Format(message, args));
        }

        public static string[] GetErrorLog()
        {
            return errors.ToArray();
        }

        /// <summary>
        /// Clears the current error log, returning what was cleared.
        /// </summary>
        /// <returns></returns>
        public static string[] ClearErrorLog()
        {
            var swap = new ConcurrentStack<string>();
            var oldErrors = Interlocked.Exchange(ref errors, swap);
            return oldErrors.ToArray();
        }

        private static string GetTitle(string jsonContext)
        {
            Regex titleRegex = new Regex("(?<=\"title\":\")[^\"]+");
            var match = titleRegex.Match(jsonContext);
            if(match != null)
                return match.Value;
            return null;
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
