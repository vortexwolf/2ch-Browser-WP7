using System.Diagnostics;
using System.Windows.Controls;

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
            string boardName = this.NavigationContext.QueryString["board"];
            string threadNumber = this.NavigationContext.QueryString["thread"];

            this._viewModel.Load(boardName, threadNumber);

            base.OnNavigatedTo(e);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Post item clicked");
        }
    }
}