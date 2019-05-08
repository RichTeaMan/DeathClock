using DeathClock.Persistence;
using DeathClock.Tmdb.Models;
using Microsoft.EntityFrameworkCore;
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
    public class TmdbFactory : AbstractPersonFactory<TmdbPerson>
    {
        private const string POPULAR_SEARCH_QUERY = "https://api.themoviedb.org/3/person/popular?api_key={0}&language=en-US&page={1}";

        private const string PERSON_DETAIL_QUERY = "https://api.themoviedb.org/3/person/{1}?api_key={0}&language=en-US";

        private const string COMBINED_CREDIT_QUERY = "https://api.themoviedb.org/3/person/{1}/combined_credits?api_key={0}&language=en-US";

        private readonly ILogger<TmdbFactory> logger;

        private readonly WebCache webCache;

        private readonly DeathClockContext deathClockContext;

        public string ApiKey { get; set; }

        public TmdbFactory(ILogger<TmdbFactory> logger, WebCache webCache, DeathClockContext deathClockContext) : base(logger)
        {
            this.logger = logger;
            this.webCache = webCache;
            this.deathClockContext = deathClockContext;
        }

        public override async Task FindNewPersons()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new InvalidOperationException("An ApiKey must be set.");
            }

            logger.LogDebug("Start get movie person list.");
            webCache.RateLimit = new RateLimit { Interval = 10, Requests = 40 };
            int page = 1;
            int pageLimit = 2;

            logger.LogDebug("Getting existing TMDB person IDs.");
            var existingTmdbIds = deathClockContext.TmdbPersons.Select(p => p.TmdbId).ToHashSet();
            logger.LogDebug($"{existingTmdbIds.Count} existing hash set IDs loaded.");

            List<int> personIds = new List<int>();

            while (page < pageLimit)
            {
                var searchResponseDocument = await webCache.GetWebPageAsync(string.Format(POPULAR_SEARCH_QUERY, ApiKey, page));
                var searchResponse = JsonConvert.DeserializeObject<PersonSearchResult>(searchResponseDocument.GetContents());

                pageLimit = searchResponse.TotalPages;
                page++;
                personIds.AddRange(searchResponse.Results.Select(r => r.Id));
            }

            personIds = personIds.Distinct().Where(id => !existingTmdbIds.Contains(id)).ToList();

            Console.WriteLine($"{personIds.Count} person IDs loaded.");

            ConcurrentBag<PersonCredits> personDetailList = new ConcurrentBag<PersonCredits>();

            int personCount = 0;
            foreach (var id in personIds)
            {
                try
                {
                    var personDetailDocument = await webCache.GetWebPageAsync(string.Format(PERSON_DETAIL_QUERY, ApiKey, id));
                    var personDetailResponse = JsonConvert.DeserializeObject<PersonDetail>(personDetailDocument.GetContents());

                    var combinedCreditDocument = await webCache.GetWebPageAsync(string.Format(COMBINED_CREDIT_QUERY, ApiKey, id));
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

            var birtlessPeople = personDetailList.Where(p => !p.PersonDetail.Birthday.HasValue);
            logger.LogDebug("Following names do not have a birthday:");
            foreach (var birthless in birtlessPeople)
            {
                logger.LogDebug($"    {birthless.PersonDetail.Name}");
            }

            var persistencePersonList = personDetailList.Where(p => p.PersonDetail.Birthday.HasValue).Select(p => Map(p)).ToArray();

            await deathClockContext.TmdbPersons.AddRangeAsync(persistencePersonList);
            await deathClockContext.SaveChangesAsync();

            Console.WriteLine("Person detail complete");
            logger.LogDebug("Ended get movie person list.");
        }

        protected override async Task<IEnumerable<TmdbPerson>> FetchExistingPersons()
        {
            var persons = await deathClockContext.TmdbPersons.ToArrayAsync();
            return persons;
        }

        protected override async Task StoreUpdatedPersons(IEnumerable<TmdbPerson> personsToUpdate)
        {
            await deathClockContext.SaveChangesAsync();
            logger.LogDebug("Save complete");
        }

        protected override async Task<bool> UpdatePerson(TmdbPerson person)
        {
            var personDetailDocument = await webCache.GetWebPageAsync(string.Format(PERSON_DETAIL_QUERY, ApiKey, person.TmdbId), person.UpdateDate);
            var personDetailResponse = JsonConvert.DeserializeObject<PersonDetail>(personDetailDocument.GetContents());

            var combinedCreditDocument = await webCache.GetWebPageAsync(string.Format(COMBINED_CREDIT_QUERY, ApiKey, person.TmdbId), person.UpdateDate);
            var combinedCreditResponse = JsonConvert.DeserializeObject<CombinedCredits>(combinedCreditDocument.GetContents());

            PersonCredits personCredits = new PersonCredits
            {
                PersonDetail = personDetailResponse,
                CombinedCredits = combinedCreditResponse
            };

            var updatedPerson = Map(personCredits);
            person.BirthDate = updatedPerson.BirthDate;
            person.DeathDate = updatedPerson.DeathDate;
            person.IsDead = updatedPerson.IsDead;
            person.Title = updatedPerson.Title;
            person.Url = updatedPerson.Url;
            person.TmdbId = updatedPerson.TmdbId;
            person.KnownFor = updatedPerson.KnownFor;
            person.DataSet = updatedPerson.DataSet;
            person.Popularity = updatedPerson.Popularity;
            person.RecordedDate = updatedPerson.RecordedDate;
            person.UpdateDate = updatedPerson.UpdateDate;

            return true;
        }

        private TmdbPerson Map(PersonCredits personCredits)
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

            string url = string.Empty;
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

            var person = new TmdbPerson
            {
                BirthDate = birthday,
                DeathDate = personDetail.DeathDay,
                IsDead = personDetail.DeathDay != null,
                Title = personDetail.Name,
                Url = url,
                TmdbId = personCredits.PersonDetail.Id,
                KnownFor = $"{personDetail.KnownForDepartment}; {knownFor}",
                DataSet = Constants.TMDB_DATASET_NAME,
                Popularity = personDetail.Popularity,
                RecordedDate = DateTime.Now,
                UpdateDate = CreatedUpdatedDate()
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
