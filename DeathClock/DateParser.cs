using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DeathClock
{
    public class DateParser
    {
        private List<DateParserDefinition> countedDateParserDefinitions = new List<DateParserDefinition>();

        private int timesUsed = 0;

        public DateParser AddDateParser(string regularExpression, params string[] dateFormats)
        {
            var countedDateParserDefintion = new DateParserDefinition(regularExpression, dateFormats);
            countedDateParserDefinitions.Add(countedDateParserDefintion);
            return this;
        }

        public DateTime? ExtractDate(string content)
        {
            var localDefinitions = countedDateParserDefinitions;
            DateTime? extractedDate = null;
            foreach (var parser in localDefinitions)
            {
                extractedDate = parser.GetDate(content);
                if (extractedDate != null)
                    break;
            }

            if (Interlocked.Increment(ref timesUsed) % 100 == 0)
            {
                countedDateParserDefinitions = localDefinitions.OrderByDescending(d => d.TimesUsed).ToList();
            }

            return extractedDate;
        }

        private class DateParserDefinition
        {
            public Regex DateRegex { get; private set; }
            public string[] DateFormats { get; private set; }

            private int timesUsed = 0;
            public int TimesUsed { get { return timesUsed; } }

            public DateParserDefinition(string regex, params string[] dateFormats)
            {
                DateRegex = new Regex(regex, RegexOptions.Compiled);
                DateFormats = dateFormats;
            }

            public virtual DateTime? GetDate(string content)
            {
                var match = DateRegex.Match(content);
                if (match.Success)
                {
                    try
                    {
                        var value = AbbreviationFix(match.Value).Replace("\\n", "").Trim();
                        var date = DateTime.ParseExact(value, DateFormats, null);
                        Interlocked.Increment(ref timesUsed);
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
                match = match.Replace("1st", "1");
                match = match.Replace("nd", string.Empty);
                match = match.Replace("rd", string.Empty);
                match = match.Replace("th", string.Empty);
                return match;
            }
        }
    }
}
