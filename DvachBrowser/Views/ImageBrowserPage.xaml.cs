using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Resources;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DvachBrowser.Views
{
    public partial class ImageBrowserPage : PhoneApplicationPage
    {
        private readonly ImageBrowserViewModel _viewModel;

        private bool _isLoaded;

        public ImageBrowserPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new ImageBrowserViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this._isLoaded)
            {
                string uriStr = this.NavigationContext.QueryString[Constants.QueryStringUri];

                this._viewModel.Init(uriStr);

                this.browser.Navigate(new Uri(uriStr));

                this._isLoaded = true;
            }

            base.OnNavigatedTo(e);
        }

        private void BeforeBrowserNavigate(object sender, NavigatingEventArgs e)
        {
            this._viewModel.ShowLoading();
        }

        private void AfterBrowserNavigate(object sender, NavigationEventArgs e)
        {
            this._viewModel.HideLoading();
        }

        private void OnBrowserError(object sender, NavigationFailedEventArgs e)
        {
            this._viewModel.ShowError(ErrorMessages.ImageBrowserLoadError);
        }

        private void OnDownloadClick(object sender, EventArgs e)
        {
            this._viewModel.Download();
        }
    }
}