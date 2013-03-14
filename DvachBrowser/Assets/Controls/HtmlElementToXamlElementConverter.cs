using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlElementToXamlElementConverter
    {
        private static readonly Dictionary<string, string> MapTags =
            new Dictionary<string, string>()
            {
                { "p", "Span" },
                { "div", "Span" },
                { "b", "Bold" },
                { "strong", "Bold" },
                { "em", "Italic" },
                { "i", "Italic" }
            };

        public string ConvertHtmlToXamlString(XDocument html)
        {
            XElement paragraph = this.ConvertHtmlToParagraph(html);
            string xaml = "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" + paragraph.ToString(SaveOptions.DisableFormatting) + "</Section>";

            return xaml;
        }

        public XElement ConvertHtmlToParagraph(XDocument html)
        {
            var xamlElements = this.ConvertHtmlEntitiesToXaml(html.Root.Nodes());

            var paragraph = new XElement("Paragraph", xamlElements);

            return paragraph;
        }

        public ICollection<XNode> ConvertHtmlEntitiesToXaml(IEnumerable<XNode> elements)
        {
            return elements.Select(el => this.ConvertHtmlEntityToXaml(el)).ToList();
        }

        public XNode ConvertHtmlEntityToXaml(XNode node)
        {
            if (node is XText)
            {
                return node;
            }

            var element = node as XElement;
            if (element == null)
            {
                return null;
            }

            if (element.Name == "br")
            {
                return new XElement("LineBreak");
            }

            string tagName = element.Name.LocalName;
            var innerXaml = this.ConvertHtmlEntitiesToXaml(element.Nodes());

            if (MapTags.ContainsKey(tagName))
            {
                return new XElement(MapTags[tagName], innerXaml);
            }

            if (element.Name == "a")
            {
                return this.CreateHyperlink(element, innerXaml);
            }

            return new XText(element.Value);
        }

        private XElement CreateHyperlink(XElement element, ICollection<XNode> innerXaml)
        {
            var href = element.Attribute("href");
            return new XElement("Hyperlink", new XAttribute("NavigateUri", href.Value), new XAttribute("TargetName", "_blank"), innerXaml);
        }
    }
}
