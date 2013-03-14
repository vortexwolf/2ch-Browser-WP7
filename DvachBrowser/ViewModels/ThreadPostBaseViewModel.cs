using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Resources;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadPostBaseViewModel : ViewModel
    {
        protected readonly BitmapManager BitmapManager;
        
        public ThreadPostBaseViewModel(string boardName, BitmapManager bitmapManager)
        {
            this.BoardName = boardName;
            this.BitmapManager = bitmapManager;
        }
        
        public virtual void MapModel(PostItemModel post)
        {
            this.Number = post.Number;
            this.HasSubject = !string.IsNullOrEmpty(post.Subject);
            this.Subject = post.Subject;
            this.Comment = post.Comment;
            this.HasImage = !string.IsNullOrEmpty(post.ThumbnailUri);
            this.ThumbnailUri = this.HasImage ? "http://2ch.hk/" + this.BoardName + "/" + post.ThumbnailUri : null;
            this.ImageUri = this.HasImage ? "http://2ch.hk/" + this.BoardName + "/" + post.ImageUri : null;
            this.AttachmentInfo = string.Format(Strings.DataFormat_Kb, post.ImageSize);
        }

        public string BoardName { get; private set; }

        public long Number { get; protected set; }

        public bool HasSubject { get; set; }

        public string Subject { get; protected set; }

        public string Comment { get; protected set; }
        
        public bool HasImage { get; protected set; }

        public string ThumbnailUri { get; protected set; }

        public string ImageUri { get; set; }

        public string AttachmentInfo { get; set; }

        public BitmapSource ThumbnailImage
        {
            get
            {
                return this.HasImage ? this.GetThumbnailImage() : null;
            }
        }

        private BitmapImage GetThumbnailImage()
        {
            var cachedImage = this.BitmapManager.GetCachedImage(this.ThumbnailUri);
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
                this.BitmapManager.DownloadImage(this.ThumbnailUri, () => this.RaiseThumbnaiImageChanged());
                return null;
            }
        }

        private void RaiseThumbnaiImageChanged()
        {
            this.OnPropertyChanged("ThumbnailImage");
        }
    }
}
