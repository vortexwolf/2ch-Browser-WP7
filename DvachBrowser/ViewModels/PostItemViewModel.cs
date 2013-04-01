using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using DvachBrowser.Assets;
using DvachBrowser.Models;

using GalaSoft.MvvmLight.Command;

namespace DvachBrowser.ViewModels
{
    public class PostItemViewModel : ThreadPostBaseViewModel
    {
        private readonly PostListViewModel _parent;

        public PostItemViewModel(PostListViewModel parent)
            : base(parent.BoardName)
        {
            this._parent = parent;
            this.NavigateLinkCommand = new RelayCommand<Hyperlink>(this.OnNavigateLink);
        }

        private int _index;

        public int Index
        {
            get { return this._index; }
            set
            {
                this._index = value;
                this.RaisePropertyChanged("Index");
            }
        }

        public ICommand NavigateLinkCommand { get; set; }

        public void MapModel(PostItemModel post, int index)
        {
            this.MapModel(post);

            this.Index = index;
        }

        private void OnNavigateLink(Hyperlink link)
        {
            Debug.WriteLine("OnNavigateLink clicked");
            this._parent.NavigateToLink(this, link);
        }
    }
}
