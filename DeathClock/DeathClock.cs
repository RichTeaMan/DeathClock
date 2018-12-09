using DeathClock.Persistence;
using Microsoft.Extensions.Logging;
using RichTea.WebCache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// Wiki list factory.
        /// </summary>
        private readonly WikiListFactory wikiListFactory;

        /// <summary>
        /// Web cache.
        /// </summary>
        private readonly WebCache webCache;

        private readonly JsonPersistence jsonPersistence;

        private readonly PersonMapper personMapper;

        /// <summary>
        /// Gets or sets the directory where results will be saved.
        /// </summary>
        public string OutputDirectory { get; set; } = "Results";

        public DeathClock(ILogger<DeathClock> logger, PersonFactory personFactory, WikiListFactory wikiListFactory, WebCache webCache, JsonPersistence jsonPersistence, PersonMapper personMapper)
        {
            this.logger = logger;
            this.personFactory = personFactory;
            this.wikiListFactory = wikiListFactory;
            this.webCache = webCache;
            this.jsonPersistence = jsonPersistence;
            this.personMapper = personMapper;
        }

        public async Task Start(string[] listArticles)
        {
            logger.LogTrace("Deathclock started.");
            logger.LogInformation($"Output directory: {OutputDirectory}");
            logger.LogInformation($"Cache directory: {webCache.CachePath}");

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


            // batch titles in groups of 100 and process them concurrently

            var titleQueue = new Queue<string>(titles);
            while (titleQueue.Any())
            {
                var block = new List<string>();

                while (block.Count < 100 && titleQueue.TryDequeue(out string title))
                {
                    block.Add(title);
                }

                var tasks = new ConcurrentBag<Task<Person>>();
                Parallel.ForEach(block, title =>
                {
                    tasks.Add(personFactory.Create(title));
                });

                var concurrentMessage = $"{webCache.ConcurrentDownloads} concurrent downloads";
                Console.WriteLine(concurrentMessage);

                try
                {
                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    logger.LogTrace(ex, "Errors occured.");
                }

                Parallel.ForEach(tasks, personTask =>
                {
                    if (personTask.IsCompletedSuccessfully)
                    {
                        var person = personTask.Result;
                        if (person != null)
                        {
                            people.Add(person);
                        }
                        else
                        {
                            var invalidPerson = new InvalidPerson() { Name = "Unknown", Reason = "Null object. " };
                            invalidPeople.Add(invalidPerson);
                            Interlocked.Increment(ref invalids);
                        }
                    }
                    else
                    {
                        var invalidPerson = new InvalidPerson() { Name = "Exception", Reason = personTask.Exception?.Message };
                        invalidPeople.Add(invalidPerson);
                        Interlocked.Increment(ref errors);
                    }
                    int _c = Interlocked.Increment(ref count);
                    if (_c % 100 == 0)
                    {
                        var message = $"{_c} of {totals} complete. {errors} errors. {invalids} invalid articles. Download speed {webCache.DownloadSpeed} kB/s.";
                        Console.WriteLine(message);
                    }

                });
            }

            Console.WriteLine($"{people.Count} people found.");
            WriteReports(people.ToList());
            await jsonPersistence.SaveDeathClockDataAsync(personMapper.MapToDeathClockData(people), Path.Combine(OutputDirectory, "deathClockData.json"));

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
                var wikiListPages = await wikiListFactory.CreateRecursively(listTitle, 2);

                var peopleTitles = wikiListPages.SelectMany(w => w.PersonTitles).ToArray();

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
                sb.AppendLine($"<p>Average age is {persons.Average(p => p.Age)}.");
            }
            sb.Append(table.GetHtml());

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(path, sb.ToString());
        }

    }
}
