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
            var table = new HtmlTable();
            table.SetHeaders("Name", "Birth Date", "Death Date", "Age", "Death Word Count");

            foreach (var person in persons.Where(p => p.IsDead == false && p.Age < 120).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount))
            {
                table.AddRow(person.Name, person.BirthDate, person.DeathDate, person.Age, person.DeathWordCount);
            }

            var sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Death Clock</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.Append(table.GetHtml());

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
