using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DvachBrowser.Assets
{
    public class PageNavigationService
    {
        public void Navigate(string uri)
        {
            var rootFrame = ((App)Application.Current).RootFrame;

            rootFrame.Navigate(new Uri(uri, UriKind.Relative));
        }
    }
}
