using DeathClock.Tmdb.Models;
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

        private readonly WebCache webCache = new WebCache("TMDB") { RateLimit = new RateLimit { Interval = 10, Requests = 40 } };
        private readonly string apiKey;

        public TmdbFactory(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<IEnumerable<Persistence.Person>> GetMoviePersonList()
        {
            int page = 1;
            int pageLimit = 2;

            List<int> personIds = new List<int>();

            try
            {

                while (page < pageLimit)
                {
                    var searchResponseDocument = await webCache.GetWebPageAsync(string.Format(POPULAR_SEARCH_QUERY, apiKey, page));
                    var searchResponse = JsonConvert.DeserializeObject<PersonSearchResult>(searchResponseDocument.GetContents());

                    pageLimit = searchResponse.total_pages;
                    page++;
                    personIds.AddRange(searchResponse.results.Select(r => r.id));
                }

                personIds = personIds.Distinct().ToList();

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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                Console.WriteLine("Person detail complete");

                var persistencePersonList = personDetailList.Where(p => p.PersonDetail.popularity > 5.0).Select(p => Map(p)).ToArray();
                return persistencePersonList;

            }
            catch (Exception ex)
            {
                throw new Exception("Death clock ded", ex);
            }


        }

        private Persistence.Person Map(PersonCredits personCredits)
        {
            var personDetail = personCredits.PersonDetail;
            int age = -1;
            if (personDetail.birthday != null && personDetail.deathday != null)
            {
                age = (int)(personDetail.deathday.Value - personDetail.birthday.Value).TotalDays / 365;
            }
            else if (personDetail.birthday != null && personDetail.deathday == null)
            {
                age = (int)(DateTime.Now - personDetail.birthday.Value).TotalDays / 365;
            }

            DateTime birthday = new DateTime(1900, 1, 1);
            if (personDetail.birthday != null)
            {
                birthday = personDetail.birthday.Value;
            }

            string url = null;
            if (!string.IsNullOrEmpty(personDetail.imdb_id))
            {
                url = $"https://www.imdb.com/name/{personDetail.imdb_id}";
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
                DeathDate = personDetail.deathday,
                DeathWordCount = 0,
                IsDead = personDetail.deathday != null,
                IsStub = false,
                Title = personDetail.name,
                Url = url,
                WordCount = 0,
                KnownFor = $"{personDetail.known_for_department}; {knownFor}"
            };

            return person;
        }

        class PersonCredits
        {
            public PersonDetail PersonDetail { get; set; }

            public CombinedCredits CombinedCredits { get; set; }
        }
    }
}
