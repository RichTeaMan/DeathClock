using RichTea.WebCache;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    public static class Utilities
    {

        public readonly static WebCache WebCache = new WebCache("DeathListCacheCore") { MaxConcurrentDownloads = 20 };

        public const string apiUrl = "http://en.wikipedia.org/w/api.php?format=json&action=query&titles={0}&prop=revisions&rvprop=content";
        public const string Url = "http://en.wikipedia.org/wiki/{0}";
        private readonly static string redirectContains = "#REDIRECT";
        private static Regex redirectRegex = new Regex(@"(?<=#REDIRECT \[\[)[^\]]+");

        public async static Task<string> GetPage(string title)
        {
            title = CleanTitle(title);
            string urlStr = string.Format(apiUrl, title);
            var document = await WebCache.GetWebPageAsync(urlStr);
            string contents = document.GetContents();

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
