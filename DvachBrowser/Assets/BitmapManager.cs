using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace DvachBrowser.Assets
{
    /// <summary>
    /// Downloads and caches images or their errors.
    /// </summary>
    public class BitmapManager
    {
        private const int MaxImagesNumber = 75;
        private const int RemoveExceedingImagesNumber = 25;

        private readonly Dictionary<string, BitmapImageWithLastAccess> _images = new Dictionary<string, BitmapImageWithLastAccess>();
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();
        
        public CachedBitmapImageModel GetCachedImage(string uri)
        {
            if (this._errors.ContainsKey(uri))
            {
                return new CachedBitmapImageModel() { IsCached = true, Error = this._errors[uri] };
            }

            if (this._images.ContainsKey(uri))
            {
                var imageModel = this._images[uri];
                imageModel.LastAccess = DateTime.UtcNow;

                return new CachedBitmapImageModel { IsCached = true, Image = imageModel.Image };
            }

            return new CachedBitmapImageModel { IsCached = false };
        }

        public void DownloadImage(string uri, Action onFinished)
        {
            // return immideately if the image is cached
            var cachedImage = this.GetCachedImage(uri);
            if (cachedImage.IsCached)
            {
                onFinished();
                return;
            }

            // download image
            var httpGet = new HttpGetStreamTask(uri, stream => this.OnStreamDownloaded(uri, stream, onFinished));
            httpGet.OnError = error => this.OnError(uri, error, onFinished);

            httpGet.Execute();
        }

        private void OnStreamDownloaded(string uri, MemoryStream stream, Action onFinished)
        {
            BitmapSource bitmap = null;
            try
            {
                // the special case for gif images
                if (uri.EndsWith("gif"))
                {
                    bitmap = this.DecodeGif(uri, new MemoryStream(stream.ToArray()));
                }

                // try to use the default way for all other images and for failed gif images
                if (bitmap == null)
                {
                    bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                }

                this.AddImage(uri, bitmap);
            }
            catch (Exception e)
            {
                this._errors.Add(uri, e.Message);
            }

            onFinished();
        }

        private WriteableBitmap DecodeGif(string uri, MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var gd = new GifDecoder();
            int status = gd.Read(stream);
            if (status == GifDecoder.STATUS_OK)
            {
                return gd.GetImage();
            }

            return null;
        }

        private void OnError(string uri, string error, Action onFinished)
        {
            this._errors.Add(uri, error);

            onFinished();
        }

        private void AddImage(string uri, BitmapSource bitmap)
        {
            var imageModel = new BitmapImageWithLastAccess { Image = bitmap, LastAccess = DateTime.UtcNow };

            // remove old images if their number is too high
            if (this._images.Count >= MaxImagesNumber)
            {
                var oldImagesUris = this._images.OrderBy(b => b.Value.LastAccess).Take(RemoveExceedingImagesNumber).Select(b => b.Key).ToList();
                foreach (var imageUri in oldImagesUris)
                {
                    this._images.Remove(imageUri);
                }
            }

            // add the image to cache
            this._images.Add(uri, imageModel);
        }

        private class BitmapImageWithLastAccess
        {
            public BitmapSource Image { get; set; }

            public DateTime LastAccess { get; set; }
        }
    }
}
