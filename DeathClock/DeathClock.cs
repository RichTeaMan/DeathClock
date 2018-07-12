using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace DeathClock
{
    /// <summary>
    /// Death clock.
    /// </summary>
    public class DeathClock
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<DeathClock> logger;

        /// <summary>
        /// Person factory.
        /// </summary>
        private readonly PersonFactory personFactory;

        /// <summary>
        /// Gets or sets the directory where results will be saved.
        /// </summary>
        public string OutputDirectory { get; set; } = "Results";

        public DeathClock(ILogger<DeathClock> logger, PersonFactory personFactory)
        {
            this.logger = logger;
            this.personFactory = personFactory;
        }

        public async Task Start(string[] listArticles)
        {
            logger.LogTrace("Deathclock started.");

            Directory.CreateDirectory(OutputDirectory);


            Console.WriteLine("Finding articles...");
            var titles = await FindArticleTitles(listArticles);

            File.WriteAllLines(Path.Combine(OutputDirectory, "ExaminedArticles.txt"), titles.ToArray());

            var invalidPeople = new ConcurrentBag<InvalidPerson>();

            var people = new ConcurrentBag<Person>();
            int totals = titles.Count();
            int count = 0;
            int errors = 0;
            int invalids = 0;

            Console.WriteLine($"Scanning {titles.Count()} articles.");

            foreach(var p in titles)
            {
                try
                {
                    if (p.Contains("Mone"))
                    {
                        Console.WriteLine("Mone");
                    }
                    var person = await personFactory.Create(p);
                    if (person != null)
                    {
                        people.Add(person);
                    }
                    else
                    {
                        var invalidPerson = new InvalidPerson() { Name = p, Reason = "Null object. " };
                        invalidPeople.Add(invalidPerson);
                        Interlocked.Increment(ref invalids);
                    }
                }
                catch (Exception ex)
                {
                    var invalidPerson = new InvalidPerson() { Name = p, Reason = ex.Message };
                    invalidPeople.Add(invalidPerson);
                    Interlocked.Increment(ref errors);
                }
                int _c = Interlocked.Increment(ref count);
                if (_c % 100 == 0)
                {
                    var message = $"\r{_c} of {totals} complete. {errors} errors. {invalids} invalid articles. {Utilities.WebCache.ConcurrentDownloads} concurrent downloads";
                    Console.WriteLine(message);
                }

            }

            Console.WriteLine($"{people.Count} people found.");
            WriteReports(people.ToList());

            File.WriteAllLines(Path.Combine(OutputDirectory, "InvalidPeople.txt"), invalidPeople.Select(ip => $"{ip.Name} - {ip.Reason}").ToArray());
            File.WriteAllLines(Path.Combine(OutputDirectory, "Errors.txt"), personFactory.ClearErrorLog());

        }

        /// <summary>
        /// Creates a list of article titles likely (but not guaranteed) to be people.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FindArticleTitles(string[] listArticles)
        {
            var results = new List<string>();

            foreach (var listTitle in listArticles)
            {
                var peopleTitles = await GetPeopleTitles(listTitle);
                Console.WriteLine($"{peopleTitles.Count()} articles found from '{listTitle}'.");
                results.AddRange(peopleTitles);
            }

            return results.Distinct().OrderBy(p => p).ToList();
        }

        private void WriteReports(List<Person> persons)
        {
            WriteReport(persons.Where(p => p.IsDead == false && p.IsStub).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount), "The Living Stubs", Path.Combine(OutputDirectory, "TheLivingStubs.html"));
            WriteReport(persons.Where(p => p.IsDead == false && p.IsStub == false).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount), "The Living", Path.Combine(OutputDirectory, "TheLiving.html"));
            for (int i = 1990; i <= DateTime.Now.Year; i++)
            {
                string title = $"{i} Deaths";
                WriteReport(persons.Where(p => p.DeathDate != null && p.DeathDate.Value.Year == i).OrderBy(p => p.Name), title, Path.Combine(OutputDirectory, title.Replace(" ", "") + ".html"));
            }
        }

        private void WriteReport(IEnumerable<Person> persons, string title, string path)
        {
            var table = new HtmlTable();
            table.SetHeaders("Name", "Birth Date", "Death Date", "Age", "Description", "Word Count", "Death Word Count");

            foreach (var person in persons)
            {
                string name = $"<a href=\"{person.Url}\" target=\"_blank\" >{person.Title}</a>";
                string json = $"(<a href=\"{person.JsonUrl}\" target=\"_blank\" >Json</a>)";
                table.AddRow(
                    name + " " + json, person.BirthDate.ToShortDateString(),
                    person.DeathDate != null ? person.DeathDate.Value.ToShortDateString() : " - ",
                    person.Age,
                    person.Description,
                    person.WordCount,
                    person.DeathWordCount);
            }

            var sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine($"<title>{title}</title>");
            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h1>{title}</h1>");
            sb.AppendLine($"<h3>List generated at {DateTime.Now}.</h3>");
            sb.AppendLine($"<p>Showing {persons.Count()} people.</p>");
            if (persons.Count() > 0)
            {
                sb.AppendLine("<p>Average age is {0}.", persons.Average(p => p.Age));
            }
            sb.Append(table.GetHtml());

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(path, sb.ToString());
        }

        private async Task<string[]> GetPeopleTitles(string listTitle, List<string> previousLists = null, int level = 0)
        {
            logger.LogTrace($"GetPeopleTitles started. List title: {listTitle} Level: {level}");
            if ((previousLists != null && previousLists.Contains(listTitle)) || level >= 2)
            {
                return new string[0];
            }
            try
            {
                string peoplePage = await Utilities.GetPage(listTitle);
                if (previousLists == null)
                    previousLists = new List<string>();
                previousLists.Add(listTitle);

                var peopleTitles = new List<string>();
                Regex personRegex = new Regex(@"(?<=\*[^\[]*\[\[)[^\[\]\|]+");

                var matches = personRegex.Matches(peoplePage);
                foreach (Match match in matches)
                {
                    if (!match.Value.Contains("Category") && !match.Value.Contains("List") && !match.Value.Contains(" people") && !match.Value.Contains(':'))
                    {
                        var title = match.Value.Replace(' ', '_');
                        logger.LogTrace($"Person matched: {title}");
                        peopleTitles.Add(title);
                    }
                }

                // check for other people lists in this list
                Regex listRegex = new Regex(@"(?<=\[\[)List of[^\]]+");
                var listMatches = listRegex.Matches(peoplePage);
                if (listMatches.Count > 0)
                {
                    var listNames = new List<string>();
                    foreach (Match match in listMatches)
                    {
                        listNames.Add(match.Value);
                    }
                    var titles = new ConcurrentBag<string>();
                    Parallel.ForEach(listNames, async name =>
                    {
                        if (!previousLists.Contains(name))
                        {
                            var nestedTitles = await GetPeopleTitles(name, previousLists, level + 1);
                            foreach (var title in nestedTitles)
                            {
                                logger.LogTrace($"List matched: {title}");
                                titles.Add(title);
                            }
                        }
                    });
                    peopleTitles.AddRange(titles.Distinct());
                }

                return peopleTitles.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                logger.LogError(ex, "Error in GetPeopleTitles.");
                return new string[0];
            }
            finally
            {
                logger.LogTrace($"GetPeopleTitles ended. List title: {listTitle} Level: {level}");
            }

        }
    }
}
