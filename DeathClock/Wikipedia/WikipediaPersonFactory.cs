using DeathClock.Persistence;
using DeathClock.Wikipedia.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RichTea.WebCache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeathClock
{
    /// <summary>
    /// Death clock.
    /// </summary>
    public class WikipediaPersonFactory : AbstractPersonFactory<WikipediaPerson>
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<WikipediaPersonFactory> logger;

        /// <summary>
        /// Wiki utility.
        /// </summary>
        private readonly WikiUtility wikiUtility;

        /// <summary>
        /// Wiki list factory.
        /// </summary>
        private readonly WikiListFactory wikiListFactory;

        /// <summary>
        /// Web cache.
        /// </summary>
        private readonly WebCache webCache;

        private readonly DeathClockContext deathClockContext;

        private readonly WikipediaPersonMapper personMapper;

        /// <summary>
        /// Gets or sets the directory where results will be saved.
        /// </summary>
        public string OutputDirectory { get; set; } = "Results";

        /// <summary>
        /// Gets or sets list URLs to seed persons with.
        /// </summary>
        public string[] ListArticles { get; set; }


        public WikipediaPersonFactory(ILogger<WikipediaPersonFactory> logger,
            WikiUtility wikiUtility,
            WikiListFactory wikiListFactory,
            WebCache webCache,
            DeathClockContext deathClockContext,
            WikipediaPersonMapper personMapper) : base(logger)
        {
            this.logger = logger;
            this.wikiUtility = wikiUtility;
            this.wikiListFactory = wikiListFactory;
            this.webCache = webCache;
            this.deathClockContext = deathClockContext;
            this.personMapper = personMapper;
        }

        public override async Task FindNewPersons()
        {
            if (ListArticles?.Any() != true)
            {
                logger.LogError("List articles must be set.");
            }
            logger.LogTrace("Deathclock started.");
            logger.LogInformation($"Cache directory: {webCache.CachePath}");

            Console.WriteLine("Finding articles...");
            var titles = await FindArticleTitles(ListArticles);

            var invalidPeople = new ConcurrentBag<InvalidPerson>();

            var people = new ConcurrentBag<WikipediaJsonPerson>();
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

                var tasks = new ConcurrentBag<Task<WikipediaJsonPerson>>();
                Parallel.ForEach(block, title =>
                {
                    tasks.Add(wikiUtility.Create(title));
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

            var persistencePeople = people.Select(p => personMapper.Map(p)).ToArray();
            await deathClockContext.WikipediaPersons.AddRangeAsync(persistencePeople);
            await deathClockContext.SaveChangesAsync();

            File.WriteAllLines(Path.Combine(OutputDirectory, "InvalidPeople.txt"), invalidPeople.Select(ip => $"{ip.Name} - {ip.Reason}").ToArray());
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


        protected override async Task<IEnumerable<WikipediaPerson>> FetchExistingPersons()
        {
            return await deathClockContext.WikipediaPersons.ToArrayAsync();
        }

        protected override async Task<bool> UpdatePerson(WikipediaPerson person)
        {
            var wikiPerson = await wikiUtility.CreateFromJsonUrl(person.WikipediaUrl);
            var updatedPerson = personMapper.Map(wikiPerson);

            person.BirthDate = updatedPerson.BirthDate;
            person.DeathDate = updatedPerson.DeathDate;
            person.IsDead = updatedPerson.IsDead;
            person.Title = updatedPerson.Title;
            person.WikipediaUrl = updatedPerson.WikipediaUrl;
            person.IsStub = updatedPerson.IsStub;
            person.WordCount = updatedPerson.WordCount;
            person.DeathWordCount = updatedPerson.DeathWordCount;
            person.KnownFor = updatedPerson.KnownFor;
            person.RecordedDate = updatedPerson.RecordedDate;
            person.UpdateDate = updatedPerson.UpdateDate;
            person.DataSet = updatedPerson.DataSet;

            return true;
        }

        protected override async Task StoreUpdatedPersons(IEnumerable<WikipediaPerson> personsToUpdate)
        {
            await deathClockContext.SaveChangesAsync();
            logger.LogDebug("Save complete");
        }
    }
}
