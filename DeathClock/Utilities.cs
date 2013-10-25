using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DeathClock
{
    public class Utilities
    {
        static string apiUrl = "http://en.wikipedia.org/w/api.php?format=json&action=query&titles={0}&prop=revisions&rvprop=content";

        public static string GetPage(string title)
        {
            if (!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");

            string cacheFile = string.Format("Cache/{0}.txt", title);
            if (File.Exists(cacheFile))
                return File.ReadAllText(cacheFile);

            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "DeathClock");
            string url = string.Format(apiUrl, title);
            string contents = client.DownloadString(url);

            File.WriteAllText(cacheFile, contents);
            return contents;
        }
    }
}
