using System;
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

namespace DvachBrowser.ViewModels
{
    public class PostItemViewModel : ThreadPostBaseViewModel
    {
        public PostItemViewModel() : base(null, null)
        {
        }

        public PostItemViewModel(string boardName, BitmapManager bitmapManager) : base(boardName, bitmapManager)
        {
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

        public void MapModel(PostItemModel post, int index)
        {
            this.MapModel(post);

            this.Index = index;
        }
    }
}
