using System;
using System.Collections.Generic;
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
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "DeathClock");
            string url = string.Format(apiUrl, title);
            return client.DownloadString(url);
        }
    }
}
