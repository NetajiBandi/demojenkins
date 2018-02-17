using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    internal sealed class ValuesCommaSeparator : CommaSeparator
    {
        public override void PrepareContent(StringBuilder commaSeparatedContent, HtmlNodeCollection tableDataNodes)
        {
            for (int i = 0; i < tableDataNodes.Count; i++)
            {
                if (i % 2 != 0)
                {
                    var cellValue = tableDataNodes[i].InnerText.Trim().Replace(',', ' ');
                    cellValue = Regex.Replace(cellValue, @"\t|\n|\r", "");
                    cellValue = Regex.Replace(cellValue, " {2,}", " ");
                    commaSeparatedContent.AppendFormat("{0},", cellValue);
                }
            }
        }
    }
}
