using DeathClock.Wikipedia.Models;
using Microsoft.Extensions.Logging;
using RichTea.WebCache;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    public class WikiUtility
    {
        private readonly ILogger logger;
        private readonly WebCache webCache;

        public DateParser BirthDateParser { get; private set; }
        public DateParser DeathDateParser { get; private set; }
        public Regex[] DescriptionRegexList { get; private set; }

        public string[] DeathWords { get; private set; } = new string[] { "cancer", "ill", "sick", "accident", "heart attack", "stroke" };

        private readonly static string redirectContains = "#REDIRECT";
        private readonly static Regex redirectRegex = new Regex(@"(?<=#REDIRECT \[\[)[^\]]+");

        public const string apiUrl = "https://en.wikipedia.org/w/api.php?format=json&action=query&titles={0}&prop=revisions&rvprop=content";
        public const string Url = "https://en.wikipedia.org/wiki/{0}";


        public WikiUtility(ILogger<WikiUtility> logger, WebCache webCache)
        {
            this.logger = logger;
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

        public async Task<string> GetPage(string title)
        {
            title = CleanTitle(title);
            string urlStr = string.Format(apiUrl, title);
            var document = await webCache.GetWebPageAsync(urlStr);
            string contents = document.GetContents();

            // some pages signal a redirect. The redirect should be returned instead
            if (contents.Contains(redirectContains))
            {
                var redirect = redirectRegex.Match(contents);
                if (redirect.Success)
                {
                    string redirectTitle = CleanTitle(redirect.Value);

                    if (title.ToLowerInvariant() == redirectTitle.ToLowerInvariant())
                        throw new Exception("Endless redirect loop detected.");

                    contents = await GetPage(redirectTitle);
                }
            }
            return contents;
        }

        public string CleanTitle(string title)
        {
            // title before pipe is the page that should be linked, after pipe is the text for
            // that particular link
            if (title.Contains('|'))
            {
                title = title.Substring(0, title.IndexOf('|'));
            }
            // remove #, they seem to break the API
            if (title.Contains('#'))
            {
                title = title.Substring(0, title.IndexOf('#'));
            }
            if (title.Contains('!'))
            {
                title = title.Substring(0, title.IndexOf('!')).Replace("{", "").Replace("}", "");
            }

            return title;
        }

        public async Task<WikipediaJsonPerson> Create(string title)
        {
            string jsonContent = await GetPage(title);
            return CreateFromContent(jsonContent);

        }

        public async Task<WikipediaJsonPerson> CreateFromJsonUrl(string jsonUrl)
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

        public WikipediaJsonPerson CreateFromContent(string jsonContent)
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
                    logger?.LogError($"{personTitle} has a death date but no birth date.");
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

            var person = new WikipediaJsonPerson(personName, personBirthDate, personDeathDate, personDeathWordCount, personTitle, personWordCount, personDescription);
            return person;
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
