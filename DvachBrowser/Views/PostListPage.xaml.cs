using System;
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

namespace DvachBrowser.Views
{
    public partial class PostListPage : PhoneApplicationPage
    {
        private readonly PostListViewModel _viewModel;
        private readonly ProgressIndicator _systemProgressIndicator;

        private bool _isLoaded;

        public PostListPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new PostListViewModel(this.popupPlaceholder);

            this.LocalizeAppBar();

            // update progress indicator
            this._systemProgressIndicator = new ProgressIndicator() { IsIndeterminate = false, Text = Strings.Loading };
            BindingOperations.SetBinding(this._systemProgressIndicator, ProgressIndicator.IsVisibleProperty, new Binding("IsLoadingAfterUpdate") { Source = this.DataContext });
            BindingOperations.SetBinding(this._systemProgressIndicator, ProgressIndicator.ValueProperty, new Binding("ProgressAfterUpdate") { Source = this.DataContext });
            SystemTray.SetProgressIndicator(this, this._systemProgressIndicator);
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // nothing
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
    }
}