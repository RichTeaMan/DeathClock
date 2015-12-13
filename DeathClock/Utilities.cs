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
    public static class Utilities
    {

        static string CACHE_FOLDER;

        static Utilities()
        {
            CACHE_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DeathListCache");
        }

        const string USER_AGENT = "DeathList";

        public const string apiUrl = "http://en.wikipedia.org/w/api.php?format=json&action=query&titles={0}&prop=revisions&rvprop=content";
        public const string Url = "http://en.wikipedia.org/wiki/{0}";
        static string redirectContains = "#REDIRECT";
        static Regex redirectRegex = new Regex(@"(?<=#REDIRECT \[\[)[^\]]+");

        public async static Task<string> GetPage(string title)
        {
            if (title.Contains("Guitar"))
            {
                Console.WriteLine("!!");
            }
            if (!Directory.Exists(CACHE_FOLDER))
                Directory.CreateDirectory(CACHE_FOLDER);
            title = CleanTitle(title);

            string cacheFileName = string.Format("{0}\\{1}.html", CACHE_FOLDER, title.Replace("\"", "QUOT").Replace("/", "FSLASH").Replace("\\", "BSLASH").Replace("#", "HASH"));

            string contents;
            if (File.Exists(cacheFileName))
            {
                contents = File.ReadAllText(cacheFileName);
            }
            else
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("User-Agent", USER_AGENT);
                    string urlStr = string.Format(apiUrl, title);
                    var url = new Uri(urlStr, UriKind.Absolute);
                    contents = await client.DownloadStringTaskAsync(url);
                }
                // remove comments
                Regex commentRegex = new Regex("<!--(.*?)-->");
                var comments = commentRegex.Matches(contents);
                foreach (Match comment in comments)
                {
                    contents = contents.Replace(comment.Value, string.Empty);
                }

                File.WriteAllText(cacheFileName.ToLowerInvariant(), contents);
            }

            // some pages signal a redirect. The redirect should be returned instead
            if (contents.Contains(redirectContains))
            {
                var redirect = redirectRegex.Match(contents);
                if (redirect.Success)
                {
                    string redirectTitle = CleanTitle(redirect.Value);

                    if (title.ToLowerInvariant() == redirectTitle.ToLowerInvariant())
                        throw new Exception("Endless redirect loop detected.");

                    contents = await GetPage(redirectTitle);
                }
            }
            return contents;
        }

        private static string CleanTitle(string title)
        {
            // title before pipe is the page that should be linked, after pipe is the text for
            // that particular link
            if (title.Contains('|'))
            {
                title = title.Substring(0, title.IndexOf('|'));
            }
            if (title.Contains('!'))
            {
                title = title.Substring(0, title.IndexOf('!')).Replace("{", "").Replace("}", "");
            }

            return title;
        }

        public static StringBuilder AppendLine(this StringBuilder stringBuilder, string value, params object[] arg)
        {
            return stringBuilder.AppendLine(string.Format(value, arg));
        }

    }
}
