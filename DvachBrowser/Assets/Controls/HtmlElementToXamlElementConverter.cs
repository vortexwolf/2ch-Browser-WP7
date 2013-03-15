using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlElementToXamlElementConverter
    {
        private static readonly Regex ColorStyleRegex = new Regex("color: rgb\\((\\d+), (\\d+), (\\d+)\\);");

        private readonly Dictionary<string, Func<XElement, object, XElement>> _tagsToFunctionsMap;

        public HtmlElementToXamlElementConverter()
        {
            this._tagsToFunctionsMap = new Dictionary<string, Func<XElement, object, XElement>>()
                                     {
                                         { "p", this.CreateSimpleSpan },
                                         { "div", this.CreateSimpleSpan },
                                         { "b", this.CreateSimpleBold },
                                         { "strong", this.CreateSimpleBold },
                                         { "em", this.CreateSimpleItalic },
                                         { "i", this.CreateSimpleItalic },
                                         { "a", this.CreateHyperlink },
                                         { "span", this.CreateComplexSpan },
                                         { "font", this.CreateComplexSpan }
                                     };
        }

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

            string tagName = element.Name.LocalName;
            if (tagName == "br")
            {
                return new XElement("LineBreak");
            }

            if (this._tagsToFunctionsMap.ContainsKey(tagName))
            {
                var innerXaml = this.ConvertHtmlEntitiesToXaml(element.Nodes());

                var result = this._tagsToFunctionsMap[tagName](element, innerXaml);

                return result;
            }

            return new XText(element.Value);
        }

        private XElement CreateSimpleSpan(XElement element, object content)
        {
            return new XElement("Span", content);
        }

        private XElement CreateSimpleBold(XElement element, object content)
        {
            return new XElement("Bold", content);
        }

        private XElement CreateSimpleItalic(XElement element, object content)
        {
            return new XElement("Italic", content);
        }

        private XElement CreateHyperlink(XElement element, object content)
        {
            var href = element.Attribute("href");

            return new XElement(
                "Hyperlink", 
                new XAttribute("NavigateUri", href.Value), 
                new XAttribute("TargetName", "_blank"), 
                    content);
        }

        private XElement CreateComplexSpan(XElement element, object content)
        {
            var classAttribute = element.Attribute("class");
            if (classAttribute != null)
            {
                if (classAttribute.Value == "u")
                {
                    return new XElement("Underline", content);
                }
                else if (classAttribute.Value == "unkfunc")
                {
                    return new XElement("Span", new XAttribute("Foreground", "#789922"), content);
                }
                else if (classAttribute.Value == "spoiler")
                {
                    return new XElement("Span", new XAttribute("Foreground", "#48B0FD"), content);
                }
            }

            var styleAttribute = element.Attribute("style");
            if (styleAttribute != null)
            {
                var match = ColorStyleRegex.Match(styleAttribute.Value);
                if (match.Success)
                {
                    var red = byte.Parse(match.Groups[1].Value);
                    var green = byte.Parse(match.Groups[2].Value);
                    var blue = byte.Parse(match.Groups[3].Value);

                    string hexColor = "#" + BitConverter.ToString(new[] { red, green, blue }).Replace("-", string.Empty);

                    return new XElement("Span", new XAttribute("Foreground", hexColor), content);
                }
            }

            return this.CreateSimpleSpan(element, content);
        }
    }
}
