﻿using System;
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
        static string CACHE_FOLDER
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Cache");
            }
        }
        const string USER_AGENT = "DeathClock";

        public const string apiUrl = "http://en.wikipedia.org/w/api.php?format=json&action=query&titles={0}&prop=revisions&rvprop=content";
        public const string Url = "http://en.wikipedia.org/wiki/{0}";
        static string redirectContains = "#REDIRECT";
        static Regex redirectRegex = new Regex(@"(?<=#REDIRECT \[\[)[^\]]+");

        public static string GetPage(string title)
        {
            if (!Directory.Exists(CACHE_FOLDER))
                Directory.CreateDirectory(CACHE_FOLDER);

            // title before pipe is the page that should be linked, after pipe is the text for
            // that particular link
            if(title.Contains('|'))
            {
                title = title.Substring(0, title.IndexOf('|') - 1);
            }
            if (title.Contains('!'))
            {
                title = title.Substring(0, title.IndexOf('!') - 1);
            }
            title = title.Replace("\\", "").Replace("/", "_").Replace('#', '_').Replace("{", "").Replace("}", "").Replace("\"", "").Replace(' ', '_');

            string cacheFileName = string.Format("{0}/{1}.html", CACHE_FOLDER, title);

            string contents;
            if (File.Exists(cacheFileName))
            {
                contents = File.ReadAllText(cacheFileName);
            }
            else
            {
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", USER_AGENT);
                string url = string.Format(apiUrl, title);
                contents = client.DownloadString(url);

                File.WriteAllText(cacheFileName, contents);
            }

            // some pages signal a redirect. The redirect should be returned instead
            if (contents.Contains(redirectContains))
            {
                var redirect = redirectRegex.Match(contents);
                if (redirect.Success)
                {
                    string redirectTitle = redirect.Value.Replace(' ', '_');

                    // delete required to handle Windows case-insensitve file system
                    if (title.ToLowerInvariant() == redirectTitle.ToLowerInvariant())
                        File.Delete(cacheFileName);
                    contents = GetPage(redirectTitle);
                }
            }
            return contents;
        }

        public static StringBuilder AppendLine(this StringBuilder stringBuilder, string value, params object[] arg)
        {
            return stringBuilder.AppendLine(string.Format(value, arg));
        }

    }
}
