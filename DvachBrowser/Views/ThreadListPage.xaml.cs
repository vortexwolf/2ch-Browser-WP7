using DvachBrowser.ViewModels;
using Microsoft.Phone.Controls;
using System;

namespace DvachBrowser.Views
{
    public partial class ThreadListPage : PhoneApplicationPage
    {
        private ThreadListViewModel _viewModel;

        // Constructor
        public ThreadListPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new ThreadListViewModel();
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = (ThreadItemViewModel)e.AddedItems[0];

            string queryString = string.Format("?board={0}&thread={1}", this._viewModel.BoardName, item.Number);
            this.NavigationService.Navigate(new Uri("/Views/PostListPage.xaml" + queryString, UriKind.Relative));
        }
    }
}