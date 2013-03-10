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
using System.Windows.Media.Imaging;
using DvachBrowser.Assets;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadPostBaseViewModel : ViewModel
    {
        public const int MaxCharNumber = 160;

        protected readonly BitmapManager _bitmapManager;

        public ThreadPostBaseViewModel(string boardName, BitmapManager bitmapManager)
        {
            this.BoardName = boardName;
            this._bitmapManager = bitmapManager;
        }
        
        public virtual void MapModel(PostItemModel post)
        {
            this.Number = post.Number;
            this.HasSubject = !string.IsNullOrEmpty(post.Subject);
            this.Subject = post.Subject;
            this.Comment = post.Comment;
            this.HasImage = !string.IsNullOrEmpty(post.ThumbnailUri);
            this.ThumbnailUri = this.HasImage ? "http://2ch.hk/" + this.BoardName + "/" + post.ThumbnailUri : null;

            if (this.Comment.Length > MaxCharNumber)
            {
                this.Comment = this.Comment.Substring(0, MaxCharNumber) + "...";
            }
        }

        public string BoardName { get; private set; }

        public string Number { get; protected set; }

        public bool HasSubject { get; set; }

        public string Subject { get; protected set; }

        public string Comment { get; protected set; }
        
        public bool HasImage { get; protected set; }

        public string ThumbnailUri { get; protected set; }

        public BitmapSource ThumbnailImage
        {
            get
            {
                return this.HasImage ? this.GetThumbnailImage() : null;
            }
        }

        private BitmapImage GetThumbnailImage()
        {
            var cachedImage = this._bitmapManager.GetCachedImage(this.ThumbnailUri);
            if (cachedImage.IsCached)
            {
                if (cachedImage.Error != null)
                {
                    return new BitmapImage(new Uri("/Images/alert.png", UriKind.Relative));
                }
                else
                {
                    return cachedImage.Image;
                }
            }
            else
            {
                this._bitmapManager.DownloadImage(this.ThumbnailUri, () => this.RaiseThumbnaiImageChanged());
                return null;
            }
        }

        private void RaiseThumbnaiImageChanged()
        {
            this.OnPropertyChanged("ThumbnailImage");
        }
    }
}
