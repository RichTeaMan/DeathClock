using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    public class Utilities
    {
        static string apiUrl = "http://en.wikipedia.org/w/api.php?format=json&action=query&titles={0}&prop=revisions&rvprop=content";
        static Regex redirectRegex = new Regex(@"(?<=#REDIRECT \[\[)[^\]]+");


        public static string GetPage(string title)
        {
            if (!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");

            string cacheFile = string.Format("Cache/{0}.txt", title);

            string contents;

            if (File.Exists(cacheFile))
            {
                contents = File.ReadAllText(cacheFile);
            }
            else
            {
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "DeathClock");
                string url = string.Format(apiUrl, title);
                contents = client.DownloadString(url);

                File.WriteAllText(cacheFile, contents);
            }
            var redirect = redirectRegex.Match(contents);
            if (redirect.Success)
            {
                string redirectTitle = redirect.Value.Replace(' ', '_');

                // delete required to handle Windows case-insensitve file system
                if(title.ToLowerInvariant() == redirectTitle.ToLowerInvariant())
                    File.Delete(cacheFile);
                contents = GetPage(redirectTitle);
            }
            return contents;
        }
    }
}
