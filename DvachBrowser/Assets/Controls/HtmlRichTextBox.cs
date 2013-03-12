using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlRichTextBox : Control
    {
        private RichTextBox _textBox;

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

        public HtmlRichTextBox()
        {
            this.Template = XamlReader.Load(
                @"  <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <RichTextBox x:Name=""RichTextBox"" VerticalContentAlignment=""Top"" />
                    </ControlTemplate>") as ControlTemplate;
            this.ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._textBox = (RichTextBox)GetTemplateChild("RichTextBox");
        }

        private void UpdateXaml()
        {
            string text = "<div>" + this.Text + "</div>";
            var html = XDocument.Parse(text);

            XElement paragraph = this.CreateXamlParagraph(html);
            string xaml = "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" + paragraph.ToString(SaveOptions.DisableFormatting) + "</Section>";

            this._textBox.Xaml = xaml;
        }

        private XElement CreateXamlParagraph(XDocument xDoc)
        {
            var xamlElements = this.ConvertHtmlEntitiesToXaml(xDoc.Root.Nodes());

            var paragraph = new XElement("Paragraph", xamlElements);

            return paragraph;
        }

        private ICollection<XNode> ConvertHtmlEntitiesToXaml(IEnumerable<XNode> elements)
        {
            return elements.Select(el => ConvertHtmlEntityToXaml(el)).ToList();
        }

        private XNode ConvertHtmlEntityToXaml(XNode node)
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
            var innerXaml = ConvertHtmlEntitiesToXaml(element.Nodes());

            if (MapTags.ContainsKey(tagName))
            {
                return new XElement(MapTags[tagName], innerXaml);
            }

            if (element.Name == "a")
            {
                var href = element.Attribute("href");
                return new XElement("Hyperlink", new XAttribute("NavigateUri", href.Value), new XAttribute("TargetName", "_blank"), innerXaml);
            }

            return new XText(element.Value);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HtmlRichTextBox),
            new PropertyMetadata((d, e) => ((HtmlRichTextBox)d).UpdateXaml()));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(HtmlRichTextBox), new PropertyMetadata(null, (d, e) => ((HtmlRichTextBox)d)._textBox.Foreground = (Brush)e.NewValue));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(HtmlRichTextBox), new PropertyMetadata(0d, (d, e) => ((HtmlRichTextBox)d)._textBox.FontSize = (double)e.NewValue));

    }
}
