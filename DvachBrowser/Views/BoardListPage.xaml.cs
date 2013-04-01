using System;
using System.Collections.Generic;
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

namespace DvachBrowser.Views
{
    public partial class BoardListPage : PhoneApplicationPage
    {
        private readonly BoardListViewModel _viewModel;

        public BoardListPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel = new BoardListViewModel(Container.Resolve<PageNavigationService>());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.list.SelectedItem = null;

            base.OnNavigatedTo(e);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (BoardItemViewModel)this.list.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            this._viewModel.GoToBoard(selectedItem.Code);
        }

        private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();

                this._viewModel.GoToTypedBoardCommand.Execute(null);
            }
        }
    }
}