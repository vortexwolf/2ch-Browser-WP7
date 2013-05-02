using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Extensions;
using DvachBrowser.ViewModels;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DvachBrowser.Views
{
    public partial class AddPostPage : PhoneApplicationPage
    {
        private readonly AddPostViewModel _viewModel;

        private bool _isLoaded;

        public AddPostPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new AddPostViewModel();

            this.LocalizeAppBar();
            this.AddValidationBinding();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this._viewModel.UpdateAddPostStorage();
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this._isLoaded)
            {
                string boardName = this.NavigationContext.QueryString[Constants.QueryStringBoard];
                string threadNumber = this.NavigationContext.QueryString[Constants.QueryStringThread];
                string postNumber = this.NavigationContext.QueryString[Constants.QueryStringPost];

                this._viewModel.Init(boardName, threadNumber, postNumber);

                this._isLoaded = true;
            }

            base.OnNavigatedTo(e);
        }

        private void OnSendClick(object sender, EventArgs e)
        {
            this.UpdateFocusedTextBoxBinding();

            this._viewModel.Send();
        }

        private void UpdateFocusedTextBoxBinding()
        {
            var focusedElement = FocusManager.GetFocusedElement() as TextBox; 
            if (focusedElement != null)
            {
                focusedElement.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}