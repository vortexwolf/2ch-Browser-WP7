using System;
using System.Diagnostics;
using System.Windows;

using DvachBrowser.Assets;
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
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
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
                .Add(Constants.QueryStringThread, selectedItem.Number)
                .Build();

            Container.Resolve<PageNavigationService>().Navigate(Constants.PostListPageUri + queryString);
        }
    }
}