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
using System.Windows.Shapes;
using System.Xml.Linq;
using DvachBrowser.Assets.Resources;
using GalaSoft.MvvmLight.Command;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlRichTextBox : Control
    {
        private readonly HtmlElementToXamlElementConverter _htmlConverter;
        private readonly HtmlTagsHelper _htmlTagsHelper = new HtmlTagsHelper();

        private RichTextBox _textBox;
        private ItemsControl _itemsControl;
        private List<Hyperlink> _currentHyperlinks = new List<Hyperlink>(); 

        public HtmlRichTextBox()
        {
            this.Template = XamlReader.Load(
                @"  <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <Grid>
                            <RichTextBox x:Name=""RichTextBox"" VerticalContentAlignment=""Top"" Margin=""-10,0"" />
                            <ItemsControl x:Name=""ItemsControl"" Visibility=""Collapsed"" />
                        </Grid>
                    </ControlTemplate>") as ControlTemplate;
            this.ApplyTemplate();


            this._htmlConverter = new HtmlElementToXamlElementConverter();

            if (App.Current.Resources.Contains("ThemeLinkForeground"))
            {
                this._htmlConverter.SpanValues.LinkForeground = ((SolidColorBrush)App.Current.Resources["ThemeLinkForeground"]).Color.ToString();
                this._htmlConverter.SpanValues.SpoilerForeground = ((SolidColorBrush)App.Current.Resources["ThemeSpoilerForeground"]).Color.ToString();
                this._htmlConverter.SpanValues.QuoteForeground = ((SolidColorBrush)App.Current.Resources["ThemeQuoteForeground"]).Color.ToString();
                this._htmlConverter.SpanValues.SpoilerBackground = ((SolidColorBrush)App.Current.Resources["ThemeSpoilerBackground"]).Color.ToString();
                this._htmlConverter.SpanValues.TextForeground = ((SolidColorBrush)App.Current.Resources["ThemePrimaryText"]).Color.ToString();
                this._htmlConverter.SpanValues.TextFontSize = (double)App.Current.Resources["PhoneFontSizeNormal"];
            }

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._textBox = (RichTextBox)GetTemplateChild("RichTextBox");
            this._itemsControl = (ItemsControl)GetTemplateChild("ItemsControl");
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
            text = text.Replace("<br>", "<br/>");

            if (this.MaxNumberOfSymbols != null)
            {
                text = this._htmlTagsHelper.CutText(text, this.MaxNumberOfSymbols.Value);
            }

            this.SetAndConvertHtml(text);
        }

        private void SetAndConvertHtml(string text)
        {
            int displayableTextLength = this.CalculateDisplayableTextLength();

            if (text.Length <= displayableTextLength)
            {
                // single rich text box
                this._textBox.Visibility = Visibility.Visible;
                this._itemsControl.Visibility = Visibility.Collapsed;

                this.SetTextToTextBox(this._textBox, text);
            }
            else
            {   // multiple text boxes
                this._textBox.Visibility = Visibility.Collapsed;
                this._itemsControl.Visibility = Visibility.Visible;

                var strings = this._htmlTagsHelper.SplitHtml(text, displayableTextLength);

                this._itemsControl.Items.Clear();
                foreach (var str in strings)
                {
                    var textBox = this.CreateRichTextBox();
                    this.SetTextToTextBox(textBox, str);

                    this._itemsControl.Items.Add(textBox);
                    //// For test purposes
                    ////this._itemsControl.Items.Add(new Rectangle() { Height = 3, Fill = new SolidColorBrush(Colors.Red) });
                }
            }
        }

        private RichTextBox CreateRichTextBox()
        {
            var textBox = new RichTextBox();
            textBox.Margin = new Thickness(-10, 0, -10, 0);
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.FontSize = this.FontSize;
            textBox.Foreground = this.Foreground;

            return textBox;
        }

        private void SetTextToTextBox(RichTextBox textBox, string text)
        {
            textBox.Blocks.Clear();

            try
            {
                var html = XDocument.Parse("<root>" + text + "</root>");

                string xaml = this._htmlConverter.ConvertHtmlToXamlString(html);

                Section section = (Section)XamlReader.Load(xaml);
                var block = section.Blocks[0];
                section.Blocks.RemoveAt(0);

                textBox.Blocks.Add(block);

                this.AddEvents(textBox);
            }
            catch (Exception e)
            {
                // TODO: use a plain text without html somehow
                var html = new XDocument(new XElement("root", text)); // encode tags and display as text
                string xaml = this._htmlConverter.ConvertHtmlToXamlString(html);
                textBox.Xaml = xaml;

                var errorMessage = new Span() {Foreground = new SolidColorBrush(Colors.Red),};
                errorMessage.Inlines.Add(ErrorMessages.TextDisplayError);
                var errorParagraph = new Paragraph();
                errorParagraph.Inlines.Add(errorMessage);
                textBox.Blocks.Insert(0, errorParagraph);
            }
        }

        private int CalculateDisplayableTextLength()
        {
            double fontSize = this.FontSize;
            double splitSize = 2000 * 800 * Math.Pow(fontSize, -2.2); // a formula which I calculated by testing different fonts manually

            return (int)splitSize;
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

        private void AddEvents(RichTextBox textBox)
        {
            var currentHyperlinks = this.GetAllInlines<Hyperlink>(textBox.Blocks).ToList();
            this._currentHyperlinks.AddRange(currentHyperlinks);

            foreach (var link in currentHyperlinks)
            {
                link.SetValue(HyperlinkProperties.UriProperty, link.NavigateUri);
                link.NavigateUri = null;

                link.Command = new RelayCommand<Hyperlink>(p => this.HyperlinkCommand.Execute(p));
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
