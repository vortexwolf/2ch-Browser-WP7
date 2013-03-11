using System;
using System.Collections.Generic;
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
            var bitmap = new BitmapImage();
            try
            {
                bitmap.SetSource(stream);

                this.AddImage(uri, bitmap);
            }
            catch (Exception e)
            {
                // if the downloaded stream is not an image
                this._errors.Add(uri, e.Message);
            }

            onFinished();
        }

        private void OnError(string uri, string error, Action onFinished)
        {
            this._errors.Add(uri, error);

            onFinished();
        }

        private void AddImage(string uri, BitmapImage bitmap)
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
            public BitmapImage Image { get; set; }

            public DateTime LastAccess { get; set; }
        }
    }
}
