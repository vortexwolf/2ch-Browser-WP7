using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;

using GalaSoft.MvvmLight.Command;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlRichTextBox : Control
    {
        private readonly HtmlElementToXamlElementConverter _htmlConverter;
        private readonly HtmlTagsHelper _htmlTagsHelper = new HtmlTagsHelper();

        private RichTextBox _textBox;
        private List<Hyperlink> _currentHyperlinks = new List<Hyperlink>(); 

        public HtmlRichTextBox()
        {
            this.Template = XamlReader.Load(
                @"  <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <RichTextBox x:Name=""RichTextBox"" VerticalContentAlignment=""Top"" Margin=""-10,0"" />
                    </ControlTemplate>") as ControlTemplate;
            this.ApplyTemplate();

            var spanColors = new HtmlElementToXamlElementConverter.SpanColors();
            spanColors.LinkForeground = ((SolidColorBrush)App.Current.Resources["ThemeLinkForeground"]).Color.ToString();
            spanColors.SpoilerForeground = ((SolidColorBrush)App.Current.Resources["ThemeSpoilerForeground"]).Color.ToString();
            spanColors.QuoteForeground = ((SolidColorBrush)App.Current.Resources["ThemeQuoteForeground"]).Color.ToString();

            this._htmlConverter = new HtmlElementToXamlElementConverter(spanColors);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._textBox = (RichTextBox)GetTemplateChild("RichTextBox");
        }
        
        public int? MaxNumberOfSymbols { get; set; }

        public ICommand HyperlinkCommand
        {
            get { return (ICommand)GetValue(HyperlinkCommandProperty); }
            set { this.SetValue(HyperlinkCommandProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkCommandProperty =
            DependencyProperty.Register("HyperlinkCommand", typeof(ICommand), typeof(HtmlRichTextBox), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HtmlRichTextBox), new PropertyMetadata(OnTextPropertyChanged));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(HtmlRichTextBox), new PropertyMetadata(null, (d, e) => ((HtmlRichTextBox)d)._textBox.Foreground = (Brush)e.NewValue));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(HtmlRichTextBox), new PropertyMetadata(0d, (d, e) => ((HtmlRichTextBox)d)._textBox.FontSize = (double)e.NewValue));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((HtmlRichTextBox)d).UpdateXaml();
        }

        private void UpdateXaml()
        {
            this.ClearEvents();

            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            string text = this.Text;
            if (this.MaxNumberOfSymbols != null && text.Length > this.MaxNumberOfSymbols)
            {
                text = this._htmlTagsHelper.FixTags(text.Substring(0, this.MaxNumberOfSymbols.Value)) + "...";
            }

            text = text.Replace("<br>", "<br/>");

            var html = XDocument.Parse("<div>" + text + "</div>");

            string xaml = this._htmlConverter.ConvertHtmlToXamlString(html);

            this._textBox.Xaml = xaml;

            this.AddEvents();
        }

        private void ClearEvents()
        {
            foreach (var link in this._currentHyperlinks)
            {
                link.NavigateUri = (Uri)link.GetValue(HyperlinkProperties.UriProperty);
                link.SetValue(HyperlinkProperties.UriProperty, null);

                link.CommandParameter = null;
                link.Command = null;
            }

            this._currentHyperlinks.Clear();
        }

        private void AddEvents()
        {
            this._currentHyperlinks = this.GetAllInlines<Hyperlink>(this._textBox.Blocks).ToList();

            foreach (var link in this._currentHyperlinks)
            {
                link.SetValue(HyperlinkProperties.UriProperty, link.NavigateUri);
                link.NavigateUri = null;

                link.Command = new RelayCommand<Hyperlink>((parameter) => this.HyperlinkCommand.Execute(parameter));
                link.CommandParameter = link;
            }
        }

        private IEnumerable<T> GetAllInlines<T>(ICollection<Block> blocks) where T : Inline
        {
            return blocks.SelectMany(this.GetAllInlines<T>).ToList();
        }

        private IEnumerable<T> GetAllInlines<T>(Block block) where T : Inline
        {
            var paragraph = block as Paragraph;
            if (paragraph != null)
            {
                var inlines = paragraph.Inlines.SelectMany(this.GetAllInlines<T>).ToList();

                return inlines;
            }

            return Enumerable.Empty<T>();
        }

        private IEnumerable<T> GetAllInlines<T>(Inline inline) where T : Inline
        {
            var span = inline as Span;
            if (span != null)
            {
                var inlines = span.Inlines.SelectMany(this.GetAllInlines<T>).ToList();

                foreach (var si in inlines)
                {
                    yield return si;
                }
            }

            var currentInline = inline as T;
            if (currentInline != null)
            {
                yield return currentInline;
            }
        }
    }
}
