using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                        people.Add(new Person(p));
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error creating person '{0} - {1}'.", p, ex.Message);
                    }
                    
                });

            foreach (var person in people)
            {
                Console.WriteLine(person);
            }

            var ordered = people.Where(p => p.IsDead == false).OrderByDescending(p => p.Age).ThenByDescending(p => p.DeathWordCount).ToArray();

            Console.ReadKey();
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
