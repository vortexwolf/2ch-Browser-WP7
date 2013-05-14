using System;
using System.Linq;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Extensions;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace DvachBrowser.Views
{
    public partial class ThreadListPage : PhoneApplicationPage
    {
        private readonly ThreadListViewModel _viewModel;
        private readonly PageNavigationService _pageNavigationService;

        private readonly ApplicationBarIconButton _prevButton;
        private readonly ApplicationBarIconButton _nextButton;

        private bool _isLoaded;

        public ThreadListPage()
        {
            this.InitializeComponent();
            this._prevButton = (ApplicationBarIconButton)this.ApplicationBar.Buttons[2];
            this._nextButton = (ApplicationBarIconButton)this.ApplicationBar.Buttons[3];

            this._pageNavigationService = Container.Resolve<PageNavigationService>();

            this.DataContext = this._viewModel = new ThreadListViewModel();

            this.LocalizeAppBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!this._isLoaded)
            {
                string board = this.NavigationContext.QueryString.GetValueOrDefault(Constants.QueryStringBoard, Constants.DefaultBoardName);
                string page = this.NavigationContext.QueryString.GetValueOrDefault(Constants.QueryStringPage);
                int pageNumber = !string.IsNullOrEmpty(page) ? int.Parse(page) : Constants.DefaultPage;

                this._viewModel.Load(board, pageNumber);
                this._isLoaded = true;
            }

            // clear the previous selection so that we can navigate to the same thread more than 1 time
            this.list.SelectedItem = null;

            this.UpdateNextPrevButtonsVisibility();

            base.OnNavigatedTo(e);
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedItem = (ThreadItemViewModel)this.list.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            this._pageNavigationService.NavigateToPostListPage(this._viewModel.BoardName, selectedItem.Number.ToString());
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            this._viewModel.Refresh();
        }

        private void OnBoardsButtonClick(object sender, EventArgs e)
        {
            this._pageNavigationService.Navigate(Constants.BoardListPageUri);
        }

        private void OnPreviousButtonClick(object sender, EventArgs e)
        {
            this.NavigateToPage(this._viewModel.Page - 1);
        }

        private void OnNextButtonClick(object sender, EventArgs e)
        {
            this.NavigateToPage(this._viewModel.Page + 1);
        }

        private void OnOpenInBrowserClick(object sender, EventArgs e)
        {
            var uriBuilder = Container.Resolve<DvachUrlBuilder>();
            var uri = new Uri(uriBuilder.BuildCustomUrl(this._viewModel.BoardName + "/"));

            var webBrowserTask = new WebBrowserTask() { Uri = uri };
            webBrowserTask.Show();
        }

        private void NavigateToPage(int page)
        {
            this._pageNavigationService.NavigateToThreadListPage(this._viewModel.BoardName, page);
        }

        private void UpdateNextPrevButtonsVisibility()
        {
            if (this._viewModel.Page == Constants.FirstPage)
            {
                this.ApplicationBar.Buttons.Remove(this._prevButton);
            }
            else if (this._viewModel.Page == Constants.LastPage)
            {
                this.ApplicationBar.Buttons.Remove(this._nextButton);
            }
        }
    }
}