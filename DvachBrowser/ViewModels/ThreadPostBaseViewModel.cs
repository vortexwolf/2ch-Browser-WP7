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
        private readonly DvachUrlBuilder _urlBuilder;
        private readonly YoutubeHelper _youtubeHelper;

        public ThreadPostBaseViewModel(string boardName)
        {
            this.BoardName = boardName;
            this.BitmapManager = Container.Resolve<BitmapManager>();
            this._urlBuilder = Container.Resolve<DvachUrlBuilder>();
            this._youtubeHelper = Container.Resolve<YoutubeHelper>();
        }
        
        public virtual void MapModel(PostItemModel post)
        {
            this.Number = post.Number;
            this.HasSubject = !string.IsNullOrEmpty(post.Subject);
            this.Subject = post.Subject;
            this.Comment = post.Comment;

            this.HasImage = !string.IsNullOrEmpty(post.ThumbnailUri);
            this.HasVideo = !string.IsNullOrEmpty(post.VideoHtml);
            if (this.HasImage)
            {
                this.ThumbnailUri = this._urlBuilder.BuildResourceUrl(this.BoardName, post.ThumbnailUri);
                this.AttachmentUri = this._urlBuilder.BuildResourceUrl(this.BoardName, post.ImageUri);
                this.AttachmentInfo = string.Format(Strings.DataFormat_Kb, post.ImageSize);
                if (this.AttachmentUri.EndsWith(".gif"))
                {
                    this.AttachmentInfo += " gif";
                }
            }
            else if (this.HasVideo)
            {
                string code = this._youtubeHelper.GetYouTubeCode(post.VideoHtml);
                if (code != null)
                {
                    this.ThumbnailUri = this._youtubeHelper.GetThumbnailUrl(code);
                    this.AttachmentUri = this._youtubeHelper.GetVideoUrl(code);
                    this.AttachmentInfo = "YouTube";
                }
                else
                {
                    this.HasVideo = false;
                }
            }

            this.HasAttachment = this.HasImage || this.HasVideo;
        }

        public string BoardName { get; private set; }

        public long Number { get; protected set; }

        public bool HasSubject { get; set; }

        public string Subject { get; protected set; }

        public string Comment { get; protected set; }
        
        public bool HasImage { get; protected set; }

        public bool HasVideo { get; protected set; }

        public bool HasAttachment { get; protected set; }

        public string ThumbnailUri { get; protected set; }

        public string AttachmentUri { get; protected set; }

        public string AttachmentInfo { get; protected set; }

        public BitmapSource ThumbnailImage
        {
            get
            {
                return this.HasAttachment ? this.GetThumbnailImage() : null;
            }
        }

        private BitmapSource GetThumbnailImage()
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
