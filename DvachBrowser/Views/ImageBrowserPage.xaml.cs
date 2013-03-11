using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Resources;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DvachBrowser.Views
{
    public partial class ImageBrowserPage : PhoneApplicationPage
    {
        public ImageBrowserPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string uriStr = this.NavigationContext.QueryString[Constants.QueryStringUri];
            Uri uri = new Uri(uriStr);

            ApplicationTitle.Text = uriStr;            
            this.browser.Navigate(uri);

            base.OnNavigatedTo(e);
        }

        private void BeforeBrowserNavigate(object sender, NavigatingEventArgs e)
        {
            this.loadingPanel.Visibility = Visibility.Visible;
            this.error.Visibility = Visibility.Collapsed;
            this.browser.Visibility = Visibility.Collapsed;
        }

        private void AfterBrowserNavigate(object sender, NavigationEventArgs e)
        {
            this.loadingPanel.Visibility = Visibility.Collapsed; 
            this.error.Visibility = Visibility.Collapsed;
            this.browser.Visibility = Visibility.Visible;
        }

        private void OnBrowserError(object sender, NavigationFailedEventArgs e)
        {
            this.loadingPanel.Visibility = Visibility.Collapsed;
            this.error.Visibility = Visibility.Visible;
            this.browser.Visibility = Visibility.Collapsed;

            this.error.Text = ErrorMessages.ImageBrowserLoadError;
        }
    }
}