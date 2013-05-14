using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Extensions;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DvachBrowser.Views
{
    public partial class AddPostPage : PhoneApplicationPage
    {
        private readonly AddPostViewModel _viewModel;

        private bool _isLoaded;

        public AddPostPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new AddPostViewModel();

            this.LocalizeAppBar();
            this.AddValidationBinding();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this._viewModel.UpdateAddPostStorage();
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this._isLoaded)
            {
                string boardName = this.NavigationContext.QueryString[Constants.QueryStringBoard];
                string threadNumber = this.NavigationContext.QueryString[Constants.QueryStringThread];
                string postNumber = this.NavigationContext.QueryString[Constants.QueryStringPost];

                this._viewModel.Init(boardName, threadNumber, postNumber);

                this._isLoaded = true;
            }

            base.OnNavigatedTo(e);
        }

        private void OnSendClick(object sender, EventArgs e)
        {
            this.UpdateFocusedTextBoxBinding();

            this._viewModel.Send();
        }

        private void UpdateFocusedTextBoxBinding()
        {
            var focusedElement = FocusManager.GetFocusedElement() as TextBox; 
            if (focusedElement != null)
            {
                focusedElement.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void OnMarkupButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender == this.BoldButton)
            {
                this.SetMarkupTag("b");
            }
            else if (sender == this.ItalicButton)
            {
                this.SetMarkupTag("i");
            }
            else if (sender == this.UnderlineButton)
            {
                this.SetMarkupTag("u");
            }
            else if (sender == this.StrikeButton)
            {
                this.SetMarkupTag("s");
            }
            else if (sender == this.SpoilerButton)
            {
                this.SetMarkupTag("spoiler");
            }
            else if (sender == this.QuoteButton)
            {
                this.SetMarkupQuote();
            }
        }

        private void SetMarkupTag(string tag)
        {
            int start = this.CommentText.SelectionStart;
            int length = this.CommentText.SelectionLength;

            string text = this.CommentText.Text;
            var strBefore = text.Substring(0, start);
            var strSelected = this.CommentText.SelectedText;
            var strEnd = text.Substring(start + length);

            string openTag = "[" + tag + "]";
            string closeTag = "[/" + tag + "]";

            text = strBefore + openTag + strSelected + closeTag + strEnd;
            this.CommentText.Text = text;
            this.CommentText.Focus();
            this.CommentText.Select(start + openTag.Length, length);
        }

        private void SetMarkupQuote()
        {
            string quoteSymbol = ">";
            int start = this.CommentText.SelectionStart;
            int length = this.CommentText.SelectionLength;

            string text = this.CommentText.Text;
            text = text.Insert(start, quoteSymbol);

            this.CommentText.Text = text;
            this.CommentText.Focus();
            this.CommentText.Select(start + quoteSymbol.Length, length);
        }
    }
}