using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var peopleTitle = new List<string>();

            peopleTitle.AddRange(GetPeopleTitles("List_of_English_people"));
            peopleTitle.AddRange(GetPeopleTitles("List_of_Scots"));
            peopleTitle.AddRange(GetPeopleTitles("List_of_Welsh_people"));
            peopleTitle.AddRange(GetPeopleTitles("List_of_Irish_people"));
            peopleTitle.AddRange(GetPeopleTitles("Lists_of_Americans"));

            var invalidPeople = new ConcurrentBag<string>();

            var people = new ConcurrentBag<Person>();
            Parallel.ForEach(peopleTitle.Distinct(), p =>
                {
                    
                    try
                    {
                        var person = Person.Create(p);
                        if (person != null)
                        {
                            people.Add(person);
                            Console.WriteLine("{0} added.", p);
                        }
                        else
                        {
                            invalidPeople.Add(p);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating person '{0} - {1}'.", p, ex.Message);
                    }
                    
                });

            foreach (var person in people)
            {
                Console.WriteLine(person);
            }

            
            Console.WriteLine("{0} people found.", people.Count);
            WriteReports(people.ToList());

            File.WriteAllLines("InvalidPeople.txt", invalidPeople);
            File.WriteAllLines("Erros.txt", Person.ClearErrorLog());

            Console.ReadKey();
        }

        static void WriteReports(List<Person> persons)
        {
            WriteReport(persons.Where(p => p.IsDead == false).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount), "The Living", "TheLiving.html");
            for (int i = 1990; i <= DateTime.Now.Year; i++)
            {
                string title = string.Format("{0} Deaths", i);
                WriteReport(persons.Where(p => p.DeathDate != null && p.DeathDate.Value.Year == i).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount), title, title.Replace(" ", "") + ".html");
            }
        }

        static void WriteReport(IEnumerable<Person> persons, string title, string path)
        {
            var table = new HtmlTable();
            table.SetHeaders("Name", "Birth Date", "Death Date", "Age", "Word Count", "Death Word Count");

            foreach (var person in persons)
            {
                string name = string.Format("<a href=\"{0}\">{1}</a>", person.Url, person.Title);
                string json = string.Format("(<a href=\"{0}\">Json</a>)", person.JsonUrl);
                table.AddRow(
                    name + " " + json, person.BirthDate.ToShortDateString(),
                    person.DeathDate != null ? person.DeathDate.Value.ToShortDateString() : " - ",
                    person.Age,
                    person.WordCount,
                    person.DeathWordCount);
            }

            var sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>{0}</title>", title);
            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>{0}</h1>", title);
            sb.AppendLine("<h3>List generated at {0}.</h3>", DateTime.Now);
            sb.AppendLine("<p>Showing {0} people.</p>", persons.Count());
            sb.AppendLine("<p>Average age is {0}.", persons.Average(p => p.Age));
            sb.Append(table.GetHtml());

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(path, sb.ToString());
        }

        static string[] GetPeopleTitles(string listTitle, List<string> previousLists = null, int level = 0)
        {
            if ((previousLists != null && previousLists.Contains(listTitle)) || level >= 2)
                return new string[0];
            Console.WriteLine("Getting {0}.", listTitle);
            try
            {
                string peoplePage = Utilities.GetPage(listTitle);
                if (previousLists == null)
                    previousLists = new List<string>();
                previousLists.Add(listTitle);

                var peopleTitles = new List<string>();
                Regex personRegex = new Regex(@"(?<=\*\[\[)[^\[\]\|]+");

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
                    Parallel.ForEach(listNames, name =>
                    {
                        if (!previousLists.Contains(name))
                        {
                            foreach (var title in GetPeopleTitles(name, previousLists, level + 1))
                            {
                                titles.Add(title);
                            }
                        }
                    });
                    peopleTitles.AddRange(titles.Distinct());
                }

                return peopleTitles.ToArray();
            }
            catch
            {
                return new string[0];
            }

        }
    }
}
