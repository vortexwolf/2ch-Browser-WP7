using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;

namespace DvachBrowser.Assets.Controls
{
    public class HtmlRichTextBox : Control
    {
        private readonly HtmlElementToXamlElementConverter _htmlConverter = new HtmlElementToXamlElementConverter();
        private readonly HtmlTagsHelper _htmlTagsHelper = new HtmlTagsHelper();

        private RichTextBox _textBox;

        public HtmlRichTextBox()
        {
            this.Template = XamlReader.Load(
                @"  <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <RichTextBox x:Name=""RichTextBox"" VerticalContentAlignment=""Top"" Margin=""-10,0"" />
                    </ControlTemplate>") as ControlTemplate;
            this.ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._textBox = (RichTextBox)GetTemplateChild("RichTextBox");
        }
        
        public int? MaxNumberOfSymbols { get; set; }

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
            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            string text = this.Text;
            if (this.MaxNumberOfSymbols != null && text.Length > this.MaxNumberOfSymbols)
            {
                text = this._htmlTagsHelper.FixTags(text.Substring(0, this.MaxNumberOfSymbols.Value)) + "...";
            }

            var html = XDocument.Parse("<div>" + text + "</div>");

            string xaml = this._htmlConverter.ConvertHtmlToXamlString(html);

            this._textBox.Xaml = xaml;
        }
    }
}
