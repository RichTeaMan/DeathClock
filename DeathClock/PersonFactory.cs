using RichTea.WebCache;
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
        private readonly WikiUtility wikiUtility;

        private readonly WebCache webCache;

        public DateParser BirthDateParser { get; private set; }
        public DateParser DeathDateParser { get; private set; }
        public Regex[] DescriptionRegexList { get; private set; }

        public string[] DeathWords { get; private set; } = new string[] { "cancer", "ill", "sick", "accident", "heart attack", "stroke" };

        private ConcurrentStack<string> errors = new ConcurrentStack<string>();

        private readonly static string redirectContains = "#REDIRECT";
        private readonly static Regex redirectRegex = new Regex(@"(?<=#REDIRECT \[\[)[^\]]+");

        public PersonFactory(WikiUtility wikiUtility, WebCache webCache)
        {
            this.wikiUtility = wikiUtility;
            this.webCache = webCache;

            BirthDateParser = new DateParser()
                .AddDateParser(@"(?i)(?<={{birth[ -_]date( and age|)\|((d|m)f=y(es|)\||))\d+\|\d+\|\d+",
                    "yyyy|M|d")
                .AddDateParser(@"(?i)(?<=birth date(\s+|)\|)\d+\|\d+\|\d+", "yyyy|M|d")
                .AddDateParser(@"(?i)(?<=birthdate(\s+|)\|)\d+\|\d+\|\d+", "yyyy|M|d")
                .AddDateParser(@"(?<=birth_date(\s+|)=(\s+|))[^\|<\(]+", "d MMMM yyyy", "MMMM d yyyy")
                .AddDateParser(@"(?<=birth_date\s+=\s+{{OldStyleDate\|)\d+ \w+\|\d+", "d MMMM|yyyy")
                .AddDateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy")
                .AddDateParser(@"(?<= born )[^\)]+", "d MMMM yyyy")
                .AddDateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM d yyyy")
                .AddDateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\d+", "yyyy")
                .AddDateParser(@"(?<=birth_date(\s+|)=(\s+|))\d+", "yyyy")
                .AddDateParser(@"(?i)(?<={{birth-date\|)\d+", "yyyy")
                .AddDateParser(@"(?<=birth_date(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy")
                .AddDateParser(@"(?<=DATE OF BIRTH(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy")
                .AddDateParser(@"(?<=birth_date\s+= c\. )\d+", "yyyy");

            DeathDateParser = new DateParser()
                .AddDateParser(@"(?i)(?<=death( |_)date(| and age)\s*\|((d|m)f=y(es|)\||)\s*)\d+\|\d+\|\d+", "yyyy|M|d")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<\(]+", "d MMMM yyyy", "MMMM d yyyy")
                .AddDateParser(@"(?<=death_date(\s+|)={{D\-da\|)[^\|<\(]+", "d MMMM yyyy", "MMMM d yyyy")
                .AddDateParser(@"(?<=Date of death\|)[^\]]+", "d MMMM yyyy")
                .AddDateParser(@"(?<=death-date and age\|(df=yes\|)?)[^\|\]]+", "d MMMM yyyy", "MMMM d yyyy")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<{\\]+", "d MMMM yyyy", "MMMM d yyyy")
                .AddDateParser(@"(?<=death_date =\\n)[^\\\|<{]+", "d MMMM yyyy")
                .AddDateParser(@"(?<=death date\|)\d+\|\d+\|\d+", "yyyy|M|d")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<\\]+", "MMMM d yyyy")
                .AddDateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\d+ \w+ \d+", "d MMMM yyyy")
                .AddDateParser(@"(?<= died )[^\)]+", "d MMMM yyyy")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|))[^\|<\(]+", "yyyy")
                .AddDateParser(@"(?i)(?<=death year and age\|(df=yes\|)?)[^\|\]]+", "yyyy")
                .AddDateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\d+", "yyyy")
                .AddDateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy")
                .AddDateParser(@"(?<=DATE OF DEATH(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM d yyyy")
                .AddDateParser(@"(?i)(?<=Death date\s+and age\|(|mf=yes\|))\d+\|\d+\|\d+", "yyyy|M|d", "yyyy|MM|dd")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|))\w+ \d+, \d+", "MMMM dd yyyy")
                .AddDateParser(@"(?<=death_date(\s+|)=(\s+|){{dda\|)\d+\|\d+\|\d+", "yyyy|MM|dd", "yyyy|M|d")
                .AddDateParser(@"(?<=death date and\s+(given)?\s*age\s*\|)\d+\|\d+\|\d+", "yyyy|MM|dd", "yyyy|M|dd", "yyyy|M|d")
                .AddDateParser(@"(?<=d-da\|)[^\|<\(]+", "dd MMMM yyyy", "MMMM dd yyyy")
                .AddDateParser(@"(?<=Death\-date and age\|)[^\|\]]+", "MMMM d yyyy")
                .AddDateParser(@"(?<={{Death date\|(df=y(es|)\||))\d+\|\d+\|\d+", "yyyy|M|d")
                .AddDateParser(@"(?<=disappeared_date(\s+|)=(\s+|))\w+ \d+", "MMMM yyyy")
                .AddDateParser(@"(?<=death_date = \w+ or )\w+, \d+", "MMMM yyyy")
                .AddDateParser(@"(?i)(?<=Disappeared date\s+and age\|(|mf=yes\|))\d+\|\d+\|\d+", "yyyy|M|d", "yyyy|MM|dd");

            var descriptionRegexs = new[] {
                @"(?i)(?<=SHORT DESCRIPTION[ =\|]*)[^\n|{}]+",
                @"(?<=occupation[ =]*\[\[)[^\]<]+",
                @"(?<=occupation[ =]*(\[\[)?)[^\|\]<]+",
                @"(?<=title[ =]*\[\[)[^\|\]<]+",
            };
            DescriptionRegexList = descriptionRegexs.Select(r => new Regex(r, RegexOptions.Compiled)).ToArray();
        }

        public async Task<Person> Create(string title)
        {
            string jsonContent = await wikiUtility.GetPage(title);
            return CreateFromContent(jsonContent);

        }

        public async Task<Person> CreateFromJsonUrl(string jsonUrl)
        {
            var document = await webCache.GetWebPageAsync(jsonUrl);
            string contents = document.GetContents();

            // some pages signal a redirect. The redirect should be returned instead
            if (contents.Contains(redirectContains))
            {
                throw new InvalidOperationException($"JsonUrl contains a redirect: {jsonUrl}");
            }
            return CreateFromContent(contents);
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
                personName = "Unknown";
            }

            DateTime? birth = BirthDateParser.ExtractDate(jsonContent);

            DateTime? death = DeathDateParser.ExtractDate(jsonContent);

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

            var personWordCount = jsonContent.Split(" ", StringSplitOptions.RemoveEmptyEntries).Count();

            string personDescription = "Unknown";
            foreach (var descRegex in DescriptionRegexList)
            {
                var descMatch = descRegex.Match(jsonContent);
                if (descMatch.Success)
                {
                    personDescription = descMatch.Value.Replace("\\n", "").Replace("=", "").Trim();
                    break;
                }
            }

            var person = new Person(personName, personBirthDate, personDeathDate, personDeathWordCount, personTitle, personWordCount, personDescription);
            return person;
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
