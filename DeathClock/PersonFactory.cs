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
    public class PersonFactory
    {
        private WikiUtility wikiUtility;

        public DateParser[] BirthDateParsers { get; private set; }
        public DateParser[] DeathDateParsers { get; private set; }

        public string[] DeathWords { get; private set; } = new string[] { "cancer", "ill", "sick", "accident", "heart attack", "stroke" };

        private ConcurrentStack<string> errors;

        public PersonFactory(WikiUtility wikiUtility)
        {
            this.wikiUtility = wikiUtility;

            errors = new ConcurrentStack<string>();
            BirthDateParsers = new DateParser[]
            {
                new DateParser(@"(?i)(?<={{birth[ -_]date( and age|)\|((d|m)f=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d"),
                new DateParser(@"(?i)(?<=birth date(\s+|)\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?i)(?<=birthdate(\s+|)\|)\d+\|\d+\|\d+", "yyyy|M|d"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))[^\|<\(]+", "d MMMM yyyy", "MMMM d yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy"),
                new DateParser(@"(?<= born )[^\)]+", "d MMMM yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM d yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+", "yyyy"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))\d+", "yyyy"),
                new DateParser(@"(?i)(?<={{birth-date\|)\d+", "yyyy"),
                new DateParser(@"(?<=birth_date(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy"),
                new DateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy"),
                new DateParser(@"(?<=birth_date\s+= c\. )\d+", "yyyy"),
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
                new DateParser(@"(?i)(?<=Death date\s+and age\|(|mf=yes\|))\d+\|\d+\|\d+", "yyyy|M|d", "yyyy|MM|dd"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM dd yyyy"),
                new DateParser(@"(?<=death_date(\s+|)=(\s+|){{dda\|)\d+\|\d+\|\d+", "yyyy|MM|dd", "yyyy|M|d"),
                new DateParser(@"(?<=death date and age \|)\d+\|\d+\|\d+", "yyyy|MM|dd", "yyyy|M|dd", "yyyy|M|d"),
                new DateParser(@"(?<=d-da\|)\d+ \w+ \d+", "dd MMMM yyyy"),
                new DateParser(@"(?<=Death\-date and age\|)[^\|\]]+", "MMMM d yyyy"),
                new DateParser(@"(?<={{Death date\|(df=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d"),
            };
        }
        
        public async Task<Person> Create(string title)
        {
            string jsonContent = await wikiUtility.GetPage(title);
            return CreateFromContent(jsonContent);

        }

        public Person CreateFromContent(string jsonContent)
        {
            var personTitle = GetTitle(jsonContent);

            Regex nameRegex = new Regex(@"(?<=name\s+=)[^\|]+");
            var nameMatch = nameRegex.Match(jsonContent);
            string personName;
            if (nameMatch.Success)
            {
                personName = nameMatch.Value.Replace("\\n", "");
            }
            else
            {
                //personName = title.Replace('_', ' ');
                personName = "Unknown";
            }

            DateTime? birth = ExtractDate(jsonContent, BirthDateParsers);

            DateTime? death = ExtractDate(jsonContent, DeathDateParsers);

            if (birth == null)
            {
                if (death != null)
                {
                    LogError($"{personTitle} has a death date but no birth date.");
                }
                return null;
            }
            var personBirthDate = birth.Value;
            var personDeathDate = death;

            int personDeathWordCount = 0;
            foreach (var word in DeathWords)
            {
                Regex wordRegex = new Regex(string.Format(" {0} ", word));
                personDeathWordCount += wordRegex.Matches(jsonContent).Count;
            }

            var personWordCount = jsonContent.Count(c => c == ' ');

            Regex descRegex = new Regex("(?<=SHORT DESCRIPTION[ =]*)[^\n|]+");
            var descMatch = descRegex.Match(jsonContent);
            string personDescription;
            if (descMatch.Success)
            {
                personDescription = descMatch.Value.Replace("\\n", "").Replace("=", "").Trim();
            }
            else
            {
                personDescription = "Unknown";
            }

            var person = new Person(personName, personBirthDate, personDeathDate, personDeathWordCount, personTitle, personWordCount, personDescription);
            return person;
        }

        private static DateTime? ExtractDate(string content, IEnumerable<DateParser> dateParsers)
        {
            DateTime? extractedDate = null;
            foreach (var parser in dateParsers)
            {
                extractedDate = parser.GetDate(content);
                if (extractedDate != null)
                    break;
            }

            return extractedDate;
        }

        private void LogError(string message, params object[] args)
        {
            errors.Push(string.Format(message, args));
        }

        /// <summary>
        /// Clears the current error log, returning what was cleared.
        /// </summary>
        /// <returns></returns>
        public string[] ClearErrorLog()
        {
            var swap = new ConcurrentStack<string>();
            var oldErrors = Interlocked.Exchange(ref errors, swap);
            return oldErrors.ToArray();
        }

        private static string GetTitle(string jsonContext)
        {
            Regex titleRegex = new Regex("(?<=\"title\":\")[^\"]+");
            var match = titleRegex.Match(jsonContext);
            if (match != null)
                return match.Value;
            return null;
        }

    }
}
