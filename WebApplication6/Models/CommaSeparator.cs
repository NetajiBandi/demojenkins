using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    internal abstract class CommaSeparator
    {
        public abstract void PrepareContent(StringBuilder commaSeparatedContent, HtmlNodeCollection tableDataNodes);

        public StringBuilder GetCommaSeparatedContent(HtmlDocument htmlDocument)
        {
            StringBuilder commaSeparatedContent = null;

            var tableNodes = htmlDocument.DocumentNode.SelectNodes("//table");
            if (tableNodes != null)
            {
                commaSeparatedContent = new StringBuilder();
                foreach (var tableNode in tableNodes)
                {
                    var tableRowNodes = tableNode.SelectNodes("tr");
                    if (tableRowNodes == null)
                    {
                        tableRowNodes = tableNode.SelectSingleNode("tbody").SelectNodes("tr");
                    }

                    foreach (HtmlNode tableRowNode in tableRowNodes)
                    {
                        var tableHeaderNode = tableRowNode.SelectSingleNode("th");
                        if (tableHeaderNode != null)
                        {
                            var pElement = tableHeaderNode.Element("p");
                            if (pElement != null)
                            {
                                var pText = pElement.InnerText;
                                if (pText.Contains("ENROLLMENT OF THE STUDENTS") || pText.Contains("Labs and Rooms Details") || pText.Contains("PARTICULARS OF TEACHING STAFF"))
                                {
                                    goto Outer;
                                }
                            }
                        }

                        var tableDataNodes = tableRowNode.SelectNodes("td");
                        if (tableDataNodes == null || (tableDataNodes != null && tableDataNodes.Count < 2))
                        {
                            continue;
                        }

                        PrepareContent(commaSeparatedContent, tableDataNodes);
                    }
                    Outer:
                    continue;
                }

                commaSeparatedContent.AppendFormat("\r\n");
            }

            return commaSeparatedContent;
        }
    }
}
