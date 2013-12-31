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
                    var value = DateParser.AbbreviationFix(match.Value).Replace("\\n", "").Trim();
                    var date = DateTime.ParseExact(value, DateFormat, null);
                    return date;
                }
                catch { }

            }
            return null;
        }

        private static string AbbreviationFix(string match)
        {
            match = match.Replace(", ", " ");
            match = match.Replace("Jan ", "January ");
            match = match.Replace("Feb ", "February ");
            match = match.Replace("Mar ", "March ");
            match = match.Replace("Jun ", "June ");
            match = match.Replace("Jul ", "July ");
            match = match.Replace("Aug ", "August ");
            match = match.Replace("Sep ", "September ");
            match = match.Replace("Sept ", "September ");
            match = match.Replace("Oct ", "October ");
            match = match.Replace("Nov ", "November ");
            match = match.Replace("Dec ", "December ");
            return match;
        }
    }
}
