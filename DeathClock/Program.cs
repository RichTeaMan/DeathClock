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
            var people = new ConcurrentBag<Person>();
            Parallel.ForEach(peopleTitle, p =>
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
                            int x = 0;
                            x++;
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
            WriteReport(people.ToList());

            Console.ReadKey();
        }

        static void WriteReport(IList<Person> persons)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Death Clock</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<table>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Name</th>");
            sb.AppendLine("<th>Birth Date</th>");
            sb.AppendLine("<th>Death Date</th>");
            sb.AppendLine("<th>Age</th>");
            sb.AppendLine("<th>Death Word Count</th>");
            sb.AppendLine("</tr>");

            foreach (var person in persons.Where(p => p.IsDead == false).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount))
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>{0}</td>", person.Name);
                sb.AppendLine("<td>{0}</td>", person.BirthDate);
                sb.AppendLine("<td>{0}</td>", person.DeathDate);
                sb.AppendLine("<td>{0}</td>", person.Age);
                sb.AppendLine("<td>{0}</td>", person.DeathWordCount);
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText("Report.html", sb.ToString());
        }

        static string[] GetPeopleTitles(string listTitle)
        {
            string peoplePage = Utilities.GetPage(listTitle);
            var peopleTitles = new List<string>();
            Regex personRegex = new Regex(@"(?<=\[\[)[^\[\]\|]+");

            var matches = personRegex.Matches(peoplePage);
            foreach (Match match in matches)
            {
                if (!match.Value.Contains("Category") && !match.Value.Contains("List") && !match.Value.Contains(" people"))
                    peopleTitles.Add(match.Value.Replace(' ', '_'));
            }
            return peopleTitles.ToArray();
        }

        
    }
}
