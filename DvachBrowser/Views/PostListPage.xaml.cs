using System.Diagnostics;
using System.Windows.Controls;

using DvachBrowser.Assets;
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
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string boardName = this.NavigationContext.QueryString[Constants.QueryStringBoard];
            string threadNumber = this.NavigationContext.QueryString[Constants.QueryStringThread];

            this._viewModel.Load(boardName, threadNumber);

            base.OnNavigatedTo(e);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Post item clicked");
        }
    }
}