using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace SC2LiquipediaStatistics.Utilities.Parser
{
    public static class HtmlExtension
    {
        public static HtmlNodeCollection SelectSubNodes(this HtmlNode htmlNode, string xpath)
        {
            return htmlNode.SelectNodes(htmlNode.XPath + xpath);
        }

        public static HtmlNode SelectSingleSubNode(this HtmlNode htmlNode, string xpath)
        {
            return htmlNode.SelectSingleNode(htmlNode.XPath + xpath);
        }

        public static string GetCleanedInnerText(this HtmlNode htmlNode)
        {
            if (string.IsNullOrEmpty(htmlNode.InnerText))
                return null;

            var text = Regex.Replace(htmlNode.InnerText, "&.*;", string.Empty).Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }
    }
}
