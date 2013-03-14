using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlTagsHelper
    {
        public string FixTags(string html)
        {
            // the case if the text is cut in the middle of the tag
            int lastOpenTagIndex = html.LastIndexOf("<");
            if (html.LastIndexOf(">") < lastOpenTagIndex)
            {
                html = html.Substring(0, lastOpenTagIndex);
            }

            int lastAmpersandIndex = html.LastIndexOf("&");
            if (html.LastIndexOf(";") < lastAmpersandIndex)
            {
                html = html.Substring(0, lastAmpersandIndex);
            }

            // add the equal number of closed tags for open tags
            var tags = new Regex("<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+(?<!/\\s*)>").Matches(html).OfType<Match>().Select(m => m.Value).ToList();

            var closedTags = tags.Where(t => t.StartsWith("</")).ToList();
            var openTags = this.ExceptWithoutDistinct(tags, closedTags);

            if (openTags.Count > closedTags.Count)
            {
                var closedTagRegex = new Regex("</([a-z0-9]+).*?>");
                var openTagRegex = new Regex("<([a-z0-9]+).*?>");

                var closedTagNames = closedTags.Select(t => closedTagRegex.Match(t).Groups[1].Value).ToList();
                var openTagNames = openTags.Select(t => openTagRegex.Match(t).Groups[1].Value).ToList();

                List<string> appendedTagNames = this.ExceptWithoutDistinct(openTagNames, closedTagNames).Reverse().ToList();
                string appendedClosedTags = string.Concat(appendedTagNames.Select(tn => "</" + tn + ">"));

                return html + appendedClosedTags;
            }

            return html;
        }

        /// <summary>
        /// The same as Expect, but this method doesn't remove duplicates.
        /// </summary>
        private ICollection<string> ExceptWithoutDistinct(IEnumerable<string> items, IEnumerable<string> exclude)
        {
            var excludeList = exclude.ToList();
            var result = new List<string>();

            foreach (var item in items)
            {
                var matched = excludeList.FirstOrDefault(exc => exc == item);
                if (matched != null)
                {
                    excludeList.Remove(matched);
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
