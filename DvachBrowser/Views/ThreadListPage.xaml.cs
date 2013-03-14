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
        private readonly PageNavigationService _pageNavigationService;

        private bool _isLoaded;

        public ThreadListPage()
        {
            this.InitializeComponent();

            this._pageNavigationService = Container.Resolve<PageNavigationService>();

            this.DataContext = this._viewModel = new ThreadListViewModel();

            this.LocalizeAppBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!this._isLoaded)
            {
                string board = this.NavigationContext.QueryString.GetValueOrDefault(Constants.QueryStringBoard)
                               ?? Constants.DefaultBoardName;

                this._viewModel.Load(board);
                this._isLoaded = true;
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

            this._pageNavigationService.Navigate(Constants.PostListPageUri + queryString);
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            this._viewModel.Refresh();
        }

        private void OnBoardsButtonClick(object sender, EventArgs e)
        {
            this._pageNavigationService.Navigate(Constants.BoardListPageUri);
        }
    }
}