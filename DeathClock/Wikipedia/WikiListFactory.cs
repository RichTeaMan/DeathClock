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

        /// <summary>
        /// Wiki utility.
        /// </summary>
        private readonly WikiUtility wikiUtility;

        /// <summary>
        /// Gets an array of strings that are not allowed in person titles.
        /// </summary>
        public readonly string[] RestrictedPersonTokens = new[] { "Category", "List", " people", ":", "#" };

        public WikiListFactory(ILogger<WikiListFactory> logger, WikiUtility wikiUtility)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.wikiUtility = wikiUtility;
        }

        public async Task<WikiListPage> Create(string title)
        {
            logger.LogTrace($"GetPeopleTitles started. List title: {title}.");
            string jsonContent = await wikiUtility.GetPageFromTitle(title);
            return CreateFromContent(jsonContent);

        }

        public WikiListPage CreateFromContent(string jsonContent)
        {
            string listTitle = new Regex("(?<=title\"\\s*:\\s*\")[^\"]+").Match(jsonContent).Value;
            try
            {
                var personTitles = new List<string>();
                Regex personRegex = new Regex(@"(?<=\*\s*\w*\s*\[\[)[^\[\]\|]+");

                var matches = personRegex.Matches(jsonContent);
                foreach (Match match in matches)
                {

                    if (!RestrictedPersonTokens.Any(t => match.Value.Contains(t)))
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

        /// <summary>
        /// Creates a list of wiki lists, recursively built from the title wiki list.
        /// </summary>
        /// <param name="title">Root wiki list title.</param>
        /// <param name="levels">Levels of recursion to go.</param>
        /// <returns></returns>
        public async Task<IEnumerable<WikiListPage>> CreateRecursively(string title, int levels)
        {
            var resultWikiLists = new List<WikiListPage>();
            var wikiListTitleSet = new HashSet<string>();
            var rootList = await Create(title);

            resultWikiLists.Add(rootList);
            wikiListTitleSet.Add(rootList.Title);

            if (levels > 0)
            {
                foreach (var listTitle in rootList.ListTitles.Where(l => !wikiListTitleSet.Contains(l)))
                {
                    var childListPages = await CreateRecursively(listTitle, levels - 1);

                    foreach (var childListPage in childListPages.Where(l => !wikiListTitleSet.Contains(l.Title)))
                    {
                        resultWikiLists.Add(childListPage);
                        wikiListTitleSet.Add(childListPage.Title);
                    }
                }
            }

            return resultWikiLists;
        }
    }
}
