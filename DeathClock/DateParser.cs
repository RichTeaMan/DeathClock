using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeathClock
{
    public class DateParser
    {
        public Regex DateRegex { get; private set; }
        public string DateFormat { get; private set; }

        public DateParser(string regex, string dateFormat)
        {
            DateRegex = new Regex(regex, RegexOptions.Compiled);
            DateFormat = dateFormat;
        }

        public DateTime? GetDate(string content)
        {
            var match = DateRegex.Match(content);
            if (match.Success)
            {
                try
                {
                    var date = DateTime.ParseExact(match.Value.Replace("\\n", "").Trim(), DateFormat, null);
                    return date;
                }
                catch { }

            }
            return null;
        }
    }
}
