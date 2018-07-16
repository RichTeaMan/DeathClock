using RichTea.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeathClock
{
    public class WikiListPage
    {
        public string Title { get; }

        public IReadOnlyCollection<string> ListTitles { get; }

        public IReadOnlyCollection<string> PersonTitles { get; }

        public WikiListPage(string title, IEnumerable<string> listTitles, IEnumerable<string> personTitles)
        {
            Title = title;
            ListTitles = listTitles?.ToArray() ?? throw new ArgumentNullException(nameof(listTitles));
            PersonTitles = personTitles?.ToArray() ?? throw new ArgumentNullException(nameof(personTitles));
        }

        public override bool Equals(object that)
        {
            var other = that as WikiListPage;
            return new EqualsBuilder<WikiListPage>(this, that)
                .Append(ListTitles, other?.ListTitles)
                .Append(PersonTitles, other?.PersonTitles)
                .AreEqual;
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder<WikiListPage>(this)
                .Append(ListTitles)
                .Append(PersonTitles)
                .HashCode;
        }

        public override string ToString()
        {
            return new ToStringBuilder<WikiListPage>(this)
                .Append(x => x.ListTitles)
                .Append(x => x.PersonTitles)
                .ToString();
        }
    }
}
