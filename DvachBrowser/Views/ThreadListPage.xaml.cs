using DvachBrowser.ViewModels;
using Microsoft.Phone.Controls;

namespace DvachBrowser.Views
{
    public partial class ThreadListPage : PhoneApplicationPage
    {
        // Constructor
        public ThreadListPage()
        {
            this.InitializeComponent();

            this.DataContext = new ThreadListViewModel();
        }
    }
}