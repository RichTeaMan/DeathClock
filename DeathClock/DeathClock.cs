using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace DeathClock
{
    /// <summary>
    /// Death clock.
    /// </summary>
    public class DeathClock
    {
        /// <summary>
        /// Gets or sets the directory where results will be saved.
        /// </summary>
        public string ResultDirectory { get; set; } = "Results";

        public async Task Start()
        {
            Directory.CreateDirectory(ResultDirectory);

            var peopleTitle = new List<string>();

            peopleTitle.AddRange(await GetPeopleTitles("List_of_English_people"));
            peopleTitle.AddRange(await GetPeopleTitles("List_of_Scots"));
            peopleTitle.AddRange(await GetPeopleTitles("List_of_Welsh_people"));
            peopleTitle.AddRange(await GetPeopleTitles("List_of_Irish_people"));
            peopleTitle.AddRange(await GetPeopleTitles("Lists_of_Americans"));

            var invalidPeople = new ConcurrentBag<InvalidPerson>();

            var people = new ConcurrentBag<Person>();
            var titles = peopleTitle.Distinct().ToArray();
            int totals = titles.Count();
            int count = 0;
            int errors = 0;
            int invalids = 0;

            Parallel.ForEach(titles, new ParallelOptions { MaxDegreeOfParallelism = 10 }, p =>
            {
                try
                {
                    var person = Person.Create(p).Result;
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
                    // Console.WriteLine("Error creating person '{0} - {1}'.", p, ex.Message);
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

            });

            foreach (var person in people)
            {
                Console.WriteLine(person);
            }

            Console.WriteLine($"{people.Count} people found.");
            WriteReports(people.ToList());

            File.WriteAllLines(Path.Combine(ResultDirectory, "InvalidPeople.txt"), invalidPeople.Select(ip => $"{ip.Name} - {ip.Reason}").ToArray());
            File.WriteAllLines(Path.Combine(ResultDirectory, "Errors.txt"), Person.ClearErrorLog());

        }

        private void WriteReports(List<Person> persons)
        {
            WriteReport(persons.Where(p => p.IsDead == false && p.IsStub).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount), "The Living Stubs", "TheLivingStubs.html");
            WriteReport(persons.Where(p => p.IsDead == false && p.IsStub == false).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount), "The Living", "TheLiving.html");
            for (int i = 1990; i <= DateTime.Now.Year; i++)
            {
                string title = $"{i} Deaths";
                WriteReport(persons.Where(p => p.DeathDate != null && p.DeathDate.Value.Year == i).OrderBy(p => p.Name), title, Path.Combine(ResultDirectory, title.Replace(" ", "") + ".html"));
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
            if ((previousLists != null && previousLists.Contains(listTitle)) || level >= 2)
                return new string[0];
            Console.WriteLine($"Getting {listTitle}.");
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
                        peopleTitles.Add(match.Value.Replace(' ', '_'));
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
                return new string[0];
            }

        }
    }
}
