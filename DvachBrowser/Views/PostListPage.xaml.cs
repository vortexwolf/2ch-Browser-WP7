﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Controls;
using DvachBrowser.Assets.Extensions;
using DvachBrowser.Assets.Resources;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace DvachBrowser.Views
{
    public partial class PostListPage : PhoneApplicationPage
    {
        private readonly PostListViewModel _viewModel;
        private readonly ProgressIndicator _systemProgressIndicator;
        private readonly IApplicationBar _listApplicationBar;
        private readonly IApplicationBar _currentItemApplicationBar;

        private bool _isLoaded;

        public PostListPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new PostListViewModel(this.popupPlaceholder);

            // app bar
            this._listApplicationBar = this.ApplicationBar;
            this._currentItemApplicationBar = this.CreateCurrentItemApplicationBar();
            this.LocalizeAppBar();

            // update progress indicator
            this._systemProgressIndicator = new ProgressIndicator() { IsIndeterminate = false, Text = Strings.Loading };
            BindingOperations.SetBinding(this._systemProgressIndicator, ProgressIndicator.IsVisibleProperty, new Binding("IsLoadingAfterUpdate") { Source = this.DataContext });
            BindingOperations.SetBinding(this._systemProgressIndicator, ProgressIndicator.ValueProperty, new Binding("ProgressAfterUpdate") { Source = this.DataContext });
            SystemTray.SetProgressIndicator(this, this._systemProgressIndicator);

            // on selected post changed
            this._viewModel.SelectedPostChanged += this.OnSelectedPostChanged;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this._isLoaded)
            {
                // refresh the page if the post was sent successfully
                var postStorage = Container.Resolve<AddPostStorage>();
                if (postStorage.IsSentSuccessfully)
                {
                    postStorage.IsSentSuccessfully = false;
                    this._viewModel.Refresh();
                }
            }

            if (!this._isLoaded)
            {
                string boardName = this.NavigationContext.QueryString[Constants.QueryStringBoard];
                string threadNumber = this.NavigationContext.QueryString[Constants.QueryStringThread];

                this._viewModel.Load(boardName, threadNumber);

                this._isLoaded = true;
            }

            base.OnNavigatedTo(e);
        }

        private void OnSelectedPostChanged(object sender, EventArgs e)
        {
            this.ApplicationBar = this._viewModel.SelectedPost != null ? this._currentItemApplicationBar : this._listApplicationBar;
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            this._viewModel.Refresh();
        }

        private void OnBoardsButtonClick(object sender, EventArgs e)
        {
            this._viewModel.NavigateBoardsPage();
        }

        private void OnAddPostClick(object sender, EventArgs e)
        {
            this._viewModel.NavigateAddPostPage();
        }

        private void OnScrollToTopClick(object sender, EventArgs e)
        {
            this.postListView.ScrollToTop();
        }

        private void OnScrollToBottomClick(object sender, EventArgs e)
        {
            this.postListView.ScrollToBottom();
        }

        private void OnOpenInBrowserClick(object sender, EventArgs e)
        {
            var uriBuilder = Container.Resolve<DvachUrlBuilder>();
            var uri = new Uri(uriBuilder.BuildResourceUrl(this._viewModel.BoardName, "res/" + this._viewModel.ThreadNumber + ".html"));

            var webBrowserTask = new WebBrowserTask() { Uri = uri };
            webBrowserTask.Show();
        }

        private void OnReplyButtonClick(object sender, EventArgs e)
        {
            this._viewModel.ReplyToSelectedPost();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            this._viewModel.SelectedPost = null;
        }

        private void OnCopyButtonClick(object sender, EventArgs e)
        {
            var comment = this._viewModel.SelectedPost.Comment ?? string.Empty;
            Clipboard.SetText(comment);
        }

        private ApplicationBar CreateCurrentItemApplicationBar()
        {
            var bar = new ApplicationBar();

            var replyButton = new ApplicationBarIconButton() { IconUri = new Uri("/Images/appbar.reply.email.png", UriKind.Relative), Text = Strings.Reply };
            replyButton.Click += this.OnReplyButtonClick;
            bar.Buttons.Add(replyButton);

            var copyButton = new ApplicationBarIconButton() { IconUri = new Uri("/Images/appbar.clipboard.png", UriKind.Relative), Text = Strings.Copy };
            copyButton.Click += this.OnCopyButtonClick;
            bar.Buttons.Add(copyButton);

            var cancelButton = new ApplicationBarIconButton() { IconUri = new Uri("/Images/appbar.cancel.png", UriKind.Relative), Text = Strings.Cancel };
            cancelButton.Click += this.OnCancelButtonClick;
            bar.Buttons.Add(cancelButton);

            return bar;
        }
    }
}