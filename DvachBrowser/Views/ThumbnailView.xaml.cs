﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using DvachBrowser.Assets;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace DvachBrowser.Views
{
    public partial class ThumbnailView : UserControl
    {
        public ThumbnailView()
        {
            this.InitializeComponent();
        }

        private void OnImageMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var vm = (ThreadPostBaseViewModel)this.DataContext;
            if (vm.HasImage)
            {
                string queryString = new QueryStringBuilder()
                    .Add(Constants.QueryStringUri, vm.AttachmentUri)
                    .Build();

                Container.Resolve<PageNavigationService>().Navigate(Constants.ImageBrowserPageUri + queryString);
            } 
            else
            {
                var webBrowserTask = new WebBrowserTask() { Uri = new Uri(vm.AttachmentUri) };
                webBrowserTask.Show();
            }
        }
    }
}
