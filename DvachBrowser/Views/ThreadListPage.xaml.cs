using System;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Extensions;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;

namespace DvachBrowser.Views
{
    public partial class ThreadListPage : PhoneApplicationPage
    {
        private readonly ThreadListViewModel _viewModel;

        public ThreadListPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new ThreadListViewModel();

            this.LocalizeAppBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!this._viewModel.IsLoadCalled)
            {
                this._viewModel.Load(Constants.DefaultBoardName);
            }

            // clear the previous selection so that we can navigate to the same thread more than 1 time
            this.list.SelectedItem = null;

            base.OnNavigatedTo(e);
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedItem = (ThreadItemViewModel)this.list.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            string queryString = new QueryStringBuilder()
                .Add(Constants.QueryStringBoard, this._viewModel.BoardName)
                .Add(Constants.QueryStringThread, selectedItem.Number.ToString())
                .Build();

            Container.Resolve<PageNavigationService>().Navigate(Constants.PostListPageUri + queryString);
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            this._viewModel.Refresh();
        }
    }
}