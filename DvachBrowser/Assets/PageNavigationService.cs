using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.Phone.Controls;

namespace DvachBrowser.Assets
{
    public class PageNavigationService
    {
        public void Navigate(string uri)
        {
            RootFrame.Navigate(new Uri(uri, UriKind.Relative));
        }

        public void GoBack()
        {
            RootFrame.GoBack();
        }

        private static PhoneApplicationFrame RootFrame
        {
            get { return ((App)Application.Current).RootFrame; }
        }
    }
}
