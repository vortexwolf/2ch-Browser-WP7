using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using DvachBrowser.ViewModels;

namespace DvachBrowser.Views
{
    public partial class PostListPage : PhoneApplicationPage
    {
        private PostListViewModel _viewModel;

        public PostListPage()
        {
            InitializeComponent();

            this.DataContext = this._viewModel = new PostListViewModel();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string boardName = this.NavigationContext.QueryString["board"];
            string threadNumber = this.NavigationContext.QueryString["thread"];

            this._viewModel.Load(boardName, threadNumber);

            base.OnNavigatedTo(e);
        }
    }
}