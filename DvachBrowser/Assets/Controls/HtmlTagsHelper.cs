using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlTagsHelper
    {
        public string CutText(string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
                text = this.FixCutHtml(text);
                text = this.FixOpenCloseHtmlTags(text) + "...";
            }

            return text;
        }

        public ICollection<string> SplitHtml(string text, int size)
        {
            string[] strs = new string[(text.Length / size) + 1];

            for (int i = 0; i < text.Length; i += size)
            {
                string str = text.Substring(i, Math.Min(size, text.Length - i));
                strs[i / size] = str;
            }

            strs = this.RedistributeHtmlStrings(strs).ToArray();

            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = this.FixOpenCloseHtmlTags(strs[i]);
            }

            return strs;
        }

        public ICollection<string> RedistributeHtmlStrings(IList<string> strs)
        {
            string[] after = new string[strs.Count];
            string[] before = new string[strs.Count];
            string[] result = new string[strs.Count];

            for (int i = 0; i < strs.Count; i++)
            {
                result[i] = strs[i];

                //// don't fix strings like "...text<tag" and "...text</tag"

                // fix strings like "...text&nb" -> ["...text", "&nb"]
                int lastAmpersandIndex = result[i].LastIndexOf("&");
                if (result[i].LastIndexOf(";") < lastAmpersandIndex)
                {
                    before[i + 1] += result[i].Substring(lastAmpersandIndex);
                    result[i] = result[i].Substring(0, lastAmpersandIndex);
                }

                // fix strings like "tag>text" -> ["tag>", "text"]
                int firstCloseBracketIndex = result[i].IndexOf(">");
                int firstOpenBracketIndex = result[i].IndexOf("<");
                if (firstCloseBracketIndex < firstOpenBracketIndex
                    || firstOpenBracketIndex == -1 && firstCloseBracketIndex != -1)
                {
                    after[i - 1] += result[i].Substring(0, firstCloseBracketIndex + 1);
                    result[i] = result[i].Substring(firstCloseBracketIndex + 1);
                }

                // fix strings like "text1</tag>text2" -> ["text1</tag>", "text2"]
                // only short strings that has less than 5% length of the full text
                firstCloseBracketIndex = result[i].IndexOf(">");
                firstOpenBracketIndex = result[i].IndexOf("<");
                if (firstOpenBracketIndex != -1 && firstOpenBracketIndex == result[i].IndexOf("</")
                    && firstOpenBracketIndex < strs[i].Length * 0.05)
                {
                    after[i - 1] += result[i].Substring(0, firstCloseBracketIndex + 1);
                    result[i] = result[i].Substring(firstCloseBracketIndex + 1);
                }
            }

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = before[i] + result[i] + after[i];

                if (result[i].StartsWith("<br"))
                {
                    result[i] = result[i].Substring(result[i].IndexOf(">") + 1);
                }
            }

            return result;
        }

        public string FixCutHtml(string html)
        {
            // fix strings like "...text<tag" -> "...text"
            int lastOpenBracketIndex = html.LastIndexOf("<");
            if (html.LastIndexOf(">") < lastOpenBracketIndex)
            {
                html = html.Substring(0, lastOpenBracketIndex);
            }

            // fix strings like "tag>text..." -> "text..."
            int firstCloseBracketIndex = html.IndexOf(">");
            int firstOpenBracketIndex = html.IndexOf("<");
            if (firstCloseBracketIndex < firstOpenBracketIndex
                    || firstOpenBracketIndex == -1 && firstCloseBracketIndex != -1)
            {
                html = html.Substring(firstCloseBracketIndex + 1);
            }

            // fix strings like "...text&nb" -> "...text"
            int lastAmpersandIndex = html.LastIndexOf("&");
            if (html.LastIndexOf(";") < lastAmpersandIndex)
            {
                html = html.Substring(0, lastAmpersandIndex);
            }

            return html;
        }

        public string FixOpenCloseHtmlTags(string html)
        {
            // add the equal number of closed tags for open tags
            var tags = new Regex("<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+(?<!/\\s*)>").Matches(html).OfType<Match>().Select(m => m.Value).ToList();

            List<string> extraOpenTags = new List<string>();
            List<string> extraCloseTags = new List<string>();
            Stack<string> openTagNamesStack = new Stack<string>();

            foreach (var tag in tags)
            {
                if (tag.EndsWith("/>"))
                {
                    continue;
                }
                else if (tag.StartsWith("</"))
                {
                    int nameLength = tag.Length - "</".Length - ">".Length;
                    string tagName = tag.Substring("</".Length, nameLength);
                    if (openTagNamesStack.Count > 0 && openTagNamesStack.Peek() == tagName)
                    {
                        openTagNamesStack.Pop();
                    }
                    else
                    {
                        extraOpenTags.Insert(0, tagName);
                    }
                }
                else
                {
                    int tagNameEnd = tag.IndexOfAny(new[] { ' ', '>' });
                    int nameLength = tagNameEnd - "<".Length;
                    string tagName = tag.Substring("<".Length, nameLength);
                    openTagNamesStack.Push(tagName);
                }
            }

            while (openTagNamesStack.Count > 0)
            {
                string tagName = openTagNamesStack.Pop();
                extraCloseTags.Add(tagName);
            }

            if (extraOpenTags.Count > 0 || extraCloseTags.Count > 0)
            {
                string appendedOpenTags = string.Concat(extraOpenTags.Select(tn => "<" + tn + ">"));
                string appendedClosedTags = string.Concat(extraCloseTags.Select(tn => "</" + tn + ">"));

                html = appendedOpenTags + html + appendedClosedTags;
            }

            return html;
        }
    }
}
