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

        private readonly Dictionary<string, Func<XElement, ICollection<XNode>, XElement>> _tagsToFunctionsMap;

        public HtmlElementToXamlElementConverter()
        {
            this._tagsToFunctionsMap = new Dictionary<string, Func<XElement, ICollection<XNode>, XElement>>()
                                     {
                                         { "p", this.CreateSimpleSpan },
                                         { "div", this.CreateSimpleSpan },
                                         { "b", this.CreateSimpleBold },
                                         { "strong", this.CreateSimpleBold },
                                         { "em", this.CreateSimpleItalic },
                                         { "i", this.CreateSimpleItalic },
                                         { "strike", this.CreateSimpleStrike },
                                         { "sup", this.CreateSimpleSuperscript },
                                         { "sub", this.CreateSimpleSubscript },
                                         { "code", this.CreateSimpleCode },
                                         { "a", this.CreateHyperlink },
                                         { "span", this.CreateComplexSpan },
                                         { "font", this.CreateComplexSpan }
                                     };

            this.SpanValues = new SpanColors();
            this.SpanValues.TextForeground = "#FFFFFF";
            this.SpanValues.TextFontSize = 20;
            this.SpanValues.LinkForeground = "#C9BE89";
            this.SpanValues.SpoilerForeground = "#48B0FD";
            this.SpanValues.SpoilerBackground = "#BBBBBB";
            this.SpanValues.QuoteForeground = "#789922";
        }

        public SpanColors SpanValues { get; private set; }

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

        private XElement CreateSimpleSpan(XElement element, ICollection<XNode> content)
        {
            return new XElement("Span", content);
        }

        private XElement CreateSimpleBold(XElement element, ICollection<XNode> content)
        {
            return new XElement("Bold", content);
        }

        private XElement CreateSimpleItalic(XElement element, ICollection<XNode> content)
        {
            return new XElement("Italic", content);
        }

        private XElement CreateSimpleStrike(XElement element, ICollection<XNode> content)
		{
            double margin = this.GetDefaultMargin();

            var root = new XElement("Grid", this.GetDefaultMarginAttribute(content),
                            new XElement("Rectangle", 
                                new XAttribute("Height", 1), 
                                new XAttribute("Fill", this.SpanValues.TextForeground), 
                                new XAttribute("Margin", "0,0,0,-" + margin)));

            return new AdaptiveTextBlockContainer(root, content);
		}

        private XElement CreateSimpleSuperscript(XElement element, ICollection<XNode> content)
        {
            double margin = this.SpanValues.TextFontSize * 0.3;

            var root = new XElement("Border", new XAttribute("Margin", "0,-" + margin + ",0,0"));

            return new AdaptiveTextBlockContainer(root, content, new XAttribute("FontSize", this.SpanValues.TextFontSize * 0.7));
        }

        private XElement CreateSimpleSubscript(XElement element, ICollection<XNode> content)
        {
            return new XElement("Span", new XAttribute("FontSize", this.SpanValues.TextFontSize * 0.7), content);
        }

        private XElement CreateSimpleCode(XElement element, ICollection<XNode> content)
        {
            return new XElement("Span", new XAttribute("FontFamily", "Consolas"), new XAttribute("FontSize", this.SpanValues.TextFontSize * 0.9), content);
        }

        private XElement CreateHyperlink(XElement element, ICollection<XNode> content)
        {
            var href = element.Attribute("href");
            string url = href != null ? href.Value : null;
            Uri uri;
            bool isValidUrl = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);

            return new XElement(
                "Hyperlink",
                new XAttribute("Foreground", this.SpanValues.LinkForeground),
                new XAttribute("NavigateUri", isValidUrl ? url : string.Empty), 
                new XAttribute("TargetName", "_blank"), 
                    content);
        }

        private XElement CreateComplexSpan(XElement element, ICollection<XNode> content)
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
                    return new XElement("Span", new XAttribute("Foreground", this.SpanValues.QuoteForeground), content);
                }
                else if (classAttribute.Value == "spoiler")
                {
                    var root = new XElement("Border", this.GetDefaultMarginAttribute(content),
                                    new XAttribute("Background", this.SpanValues.SpoilerBackground));

                    return new AdaptiveTextBlockContainer(root, content, new XAttribute("Foreground", this.SpanValues.SpoilerForeground));
                }
                else if (classAttribute.Value == "o")
                {
                    var root = new XElement("Grid", this.GetDefaultMarginAttribute(content),
                                    new XElement("Rectangle",
                                            new XAttribute("Height", 1),
                                            new XAttribute("Fill", this.SpanValues.TextForeground),
                                            new XAttribute("VerticalAlignment", "Top")));

                    return new AdaptiveTextBlockContainer(root, content);
                }
                else if (classAttribute.Value == "s")
                {
                    return this.CreateSimpleStrike(element, content);
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

        private double GetDefaultMargin()
        {
            double margin = this.SpanValues.TextFontSize / 4.0;

            return margin;
        }

        private XAttribute GetDefaultMarginAttribute(ICollection<XNode> nodes)
        {
            return new XAttribute("Margin", "0,0,0,-" + GetDefaultMargin());
        }

        private static bool HasInnerElements(ICollection<XNode> nodes)
        {
            return nodes.OfType<XElement>().Any();
        }

        private static bool HasUiElements(ICollection<XNode> nodes)
        {
            foreach (var xElement in nodes.OfType<XElement>())
            {
                if (xElement.Name == "InlineUIContainer")
                {
                    return true;
                }

                var hasUiElements = HasUiElements(xElement.Nodes().ToList());
                if (hasUiElements)
                {
                    return true;
                }
            }

            return false;
        }

        private class AdaptiveTextBlockContainer : XElement
        {
            public AdaptiveTextBlockContainer(XElement root, ICollection<XNode> content, params XAttribute[] textBlockAttributes)
                : base("InlineUIContainer")
            {
                this.Add(root);


                var attributes = new List<XAttribute>();
                attributes.AddRange(textBlockAttributes);
                attributes.Add(new XAttribute("TextWrapping", "Wrap"));

                if (!HasInnerElements(content))
                {
                    root.Add(new XElement("TextBlock", attributes, content));
                }
                else
                {
                    attributes.Add(new XAttribute("Margin", "-10,0"));
                    attributes.Add(new XAttribute("VerticalContentAlignment", "Top"));

                    root.Add(new XElement("RichTextBox", attributes, new XElement("Paragraph", content)));
                }
            }
        }

        public class SpanColors
        {
            public string TextForeground { get; set; }

            public double TextFontSize { get; set; }

            public string QuoteForeground { get; set; }

            public string LinkForeground { get; set; }

            public string SpoilerForeground { get; set; }

            public string SpoilerBackground { get; set; }
        }
    }
}
