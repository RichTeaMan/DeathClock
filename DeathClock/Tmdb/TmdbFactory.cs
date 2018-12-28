using DeathClock.Tmdb.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RichTea.WebCache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeathClock.Tmdb
{
    public class TmdbFactory
    {

        private const string POPULAR_SEARCH_QUERY = "https://api.themoviedb.org/3/person/popular?api_key={0}&language=en-US&page={1}";

        private const string PERSON_DETAIL_QUERY = "https://api.themoviedb.org/3/person/{1}?api_key={0}&language=en-US";

        private const string COMBINED_CREDIT_QUERY = "https://api.themoviedb.org/3/person/{1}/combined_credits?api_key={0}&language=en-US";

        private const double POPULARITY_THRESHOLD = 5.0;

        private readonly ILogger<TmdbFactory> logger;

        private readonly WebCache webCache;

        public TmdbFactory(ILogger<TmdbFactory> logger, WebCache webCache)
        {
            this.logger = logger;
            this.webCache = webCache;
        }

        public async Task<IEnumerable<Persistence.Person>> GetMoviePersonList(string apiKey)
        {
            logger.LogDebug("Start get movie person list.");
            webCache.RateLimit = new RateLimit { Interval = 10, Requests = 40 };
            int page = 1;
            int pageLimit = 2;

            List<int> personIds = new List<int>();
            
            try
            {

                while (page < pageLimit)
                {
                    var searchResponseDocument = await webCache.GetWebPageAsync(string.Format(POPULAR_SEARCH_QUERY, apiKey, page));
                    var searchResponse = JsonConvert.DeserializeObject<PersonSearchResult>(searchResponseDocument.GetContents());

                    pageLimit = searchResponse.TotalPages;
                    page++;
                    personIds.AddRange(searchResponse.Results.Select(r => r.Id));
                }

                personIds = personIds.Distinct().ToList();
                logger.LogDebug($"The {personIds.Count} IDs to be searched:");
                foreach(var personId in personIds)
                {
                    logger.LogDebug($"{personId}");
                }

                Console.WriteLine($"{personIds.Count} person IDs loaded.");

                ConcurrentBag<PersonCredits> personDetailList = new ConcurrentBag<PersonCredits>();

                int personCount = 0;
                foreach (var id in personIds)
                {
                    try
                    {
                        var personDetailDocument = await webCache.GetWebPageAsync(string.Format(PERSON_DETAIL_QUERY, apiKey, id));
                        var personDetailResponse = JsonConvert.DeserializeObject<PersonDetail>(personDetailDocument.GetContents());

                        var combinedCreditDocument = await webCache.GetWebPageAsync(string.Format(COMBINED_CREDIT_QUERY, apiKey, id));
                        var combinedCreditResponse = JsonConvert.DeserializeObject<CombinedCredits>(combinedCreditDocument.GetContents());

                        PersonCredits personCredits = new PersonCredits
                        {
                            PersonDetail = personDetailResponse,
                            CombinedCredits = combinedCreditResponse
                        };
                        personDetailList.Add(personCredits);

                        int count = Interlocked.Increment(ref personCount);
                        if (count % 10 == 0)
                        {
                            var message = $"{count} of {personIds.Count} complete. Download speed {webCache.DownloadSpeed} kB/s.";
                            Console.WriteLine(message);
                        }
                        logger.LogDebug($"Person '{personCredits.PersonDetail.Name}' logged. '{personCredits.PersonDetail?.Birthday?.ToShortDateString()}' - '{personCredits.PersonDetail?.DeathDay?.ToShortDateString()}'");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error while processing ID '{id}'.");
                        Console.WriteLine(ex);
                    }
                }

                var unpopularPeople = personDetailList.Where(p => p.PersonDetail.Popularity < POPULARITY_THRESHOLD);
                logger.LogDebug("Following names do not pass popularity threshold.");
                foreach(var unpopular in unpopularPeople)
                {
                    logger.LogDebug($"    {unpopular.PersonDetail.Name} - {unpopular.PersonDetail.Popularity}");
                }

                var persistencePersonList = personDetailList.Where(p => p.PersonDetail.Popularity >= POPULARITY_THRESHOLD).Select(p => Map(p)).ToArray();
                Console.WriteLine("Person detail complete");
                logger.LogDebug("Ended get movie person list.");
                return persistencePersonList;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing movie person list.");
                throw;
            }
        }

        private Persistence.Person Map(PersonCredits personCredits)
        {
            var personDetail = personCredits.PersonDetail;
            int age = -1;
            if (personDetail.Birthday != null && personDetail.DeathDay != null)
            {
                age = (int)(personDetail.DeathDay.Value - personDetail.Birthday.Value).TotalDays / 365;
            }
            else if (personDetail.Birthday != null && personDetail.DeathDay == null)
            {
                age = (int)(DateTime.Now - personDetail.Birthday.Value).TotalDays / 365;
            }

            DateTime birthday = new DateTime(1900, 1, 1);
            if (personDetail.Birthday != null)
            {
                birthday = personDetail.Birthday.Value;
            }

            string url = null;
            if (!string.IsNullOrEmpty(personDetail.ImdbId))
            {
                url = $"https://www.imdb.com/name/{personDetail.ImdbId}";
            }

            string knownFor = "Nothing";
            if (personCredits.CombinedCredits.Cast?.Count() > 0)
            {
                var popular = personCredits.CombinedCredits.Cast
                .OrderByDescending(c => c.VoteCount)
                .First();
                if (popular.Name != null)
                    knownFor = popular.Name;
                else if (popular.Title != null)
                    knownFor = popular.Title;
            }
            else if (personCredits.CombinedCredits.Crew?.Count() > 0)
            {
                var popular = personCredits.CombinedCredits.Crew
                    .OrderByDescending(c => c.VoteCount)
                    .First();
                if (popular.Name != null)
                    knownFor = popular.Name;
                else if (popular.Title != null)
                    knownFor = popular.Title;
            }

            var person = new Persistence.Person
            {
                Age = age,
                BirthDate = birthday,
                DeathDate = personDetail.DeathDay,
                DeathWordCount = 0,
                IsDead = personDetail.DeathDay != null,
                IsStub = false,
                Title = personDetail.Name,
                Url = url,
                WordCount = 0,
                KnownFor = $"{personDetail.KnownForDepartment}; {knownFor}"
            };

            return person;
        }

        private class PersonCredits
        {
            public PersonDetail PersonDetail { get; set; }

            public CombinedCredits CombinedCredits { get; set; }
        }
    }
}
