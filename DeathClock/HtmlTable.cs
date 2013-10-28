using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathClock
{
    public class HtmlTable
    {
        private string[] Headers;
        private List<string[]> Data;

        public HtmlTable()
        {
            Data = new List<string[]>();
        }

        public HtmlTable SetHeaders(params object[] arg)
        {
            var headers = new List<string>();
            foreach (var a in arg)
            {
                headers.Add(a.ToString());
            }
            Headers = headers.ToArray();
            return this;
        }

        public HtmlTable AddRow(params object[] arg)
        {
            var args = new List<string>();
            foreach (var a in arg)
            {
                if (a != null)
                    args.Add(a.ToString());
                else
                    args.Add(string.Empty);
            }
            Data.Add(args.ToArray());
            return this;
        }

        public string GetHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table>");

            if (Headers != null)
            {
                sb.AppendLine("<tr>");
                foreach (var header in Headers)
                {
                    sb.AppendLine("<th>{0}</th>", header);
                }
                sb.AppendLine("</tr>");
            }

            foreach (var row in Data)
            {
                sb.AppendLine("<tr>");
                foreach (var cell in row)
                {
                    sb.AppendLine("<td>{0}</td>", cell);
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}
