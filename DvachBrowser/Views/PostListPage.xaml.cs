using System;
using System.Diagnostics;
using System.Windows.Controls;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Extensions;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;

namespace DvachBrowser.Views
{
    public partial class PostListPage : PhoneApplicationPage
    {
        private readonly PostListViewModel _viewModel;

        public PostListPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new PostListViewModel();

            this.LocalizeAppBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!this._viewModel.IsLoadCalled)
            {
                string boardName = this.NavigationContext.QueryString[Constants.QueryStringBoard];
                string threadNumber = this.NavigationContext.QueryString[Constants.QueryStringThread];

                this._viewModel.Load(boardName, threadNumber);
            }

            base.OnNavigatedTo(e);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Post item clicked");
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            this._viewModel.Refresh();
        }
    }
}