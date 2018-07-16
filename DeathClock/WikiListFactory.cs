using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    public class WikiListFactory
    {

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<WikiListFactory> logger;

        public WikiListFactory(ILogger<WikiListFactory> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WikiListPage> Create(string title)
        {
            logger.LogTrace($"GetPeopleTitles started. List title: {title}.");
            string jsonContent = await Utilities.GetPage(title);
            return CreateFromContent(jsonContent);

        }

        public WikiListPage CreateFromContent(string jsonContent)
        {
            string listTitle = new Regex("(?<=title\": \")[\"]+").Match(jsonContent).Value;
            try
            {
                var personTitles = new List<string>();
                Regex personRegex = new Regex(@"(?<=\*\s*\[\[)[^\[\]\|]+");

                var matches = personRegex.Matches(jsonContent);
                foreach (Match match in matches)
                {
                    if (!match.Value.Contains("Category") && !match.Value.Contains("List") && !match.Value.Contains(" people") && !match.Value.Contains(':'))
                    {
                        var title = match.Value.Replace(' ', '_');
                        logger.LogTrace($"Person matched: {title}");
                        personTitles.Add(title);
                    }
                }

                // check for other people lists in this list
                Regex listRegex = new Regex(@"(?<=\[\[)List(|s) of[^|\]]+");
                var listMatches = listRegex.Matches(jsonContent);
                var listNames = new List<string>();
                foreach (Match match in listMatches)
                {
                    var subListTitle = match.Value.Replace(' ', '_');
                    listNames.Add(subListTitle);
                }

                var wikiList = new WikiListPage(listTitle, listNames.Distinct(), personTitles.Distinct());
                return wikiList;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetPeopleTitles.");
                throw ex;
            }
        }
    }
}
