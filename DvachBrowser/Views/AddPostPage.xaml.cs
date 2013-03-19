using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using DvachBrowser.Assets.Extensions;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DvachBrowser.Views
{
    public partial class AddPostPage : PhoneApplicationPage
    {
        private readonly AddPostViewModel _viewModel;

        public AddPostPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new AddPostViewModel();

            this.LocalizeAppBar();
            this.AddValidationBinding();
        }

        private void OnSendClick(object sender, EventArgs e)
        {
            this._viewModel.Send();
        }
    }
}