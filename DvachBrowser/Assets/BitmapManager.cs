using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using DvachBrowser.Assets.HttpTasks;

namespace DvachBrowser.Assets
{
    /// <summary>
    /// Downloads and caches images or their errors.
    /// </summary>
    public class BitmapManager
    {
        private const int MaxImagesNumber = 75;
        private const int RemoveExceedingImagesNumber = 25;

        private readonly List<string> _runningImageTasks = new List<string>();
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

            if (this._runningImageTasks.Contains(uri))
            {
                return;
            }

            this._runningImageTasks.Add(uri);

            // download image
            var httpGet = new HttpGetImageTask(uri, image => this.OnImageDownloaded(uri, image, onFinished));
            httpGet.OnError = error => this.OnError(uri, error, onFinished);

            httpGet.Execute();
        }

        private void OnImageDownloaded(string uri, BitmapSource bitmap, Action onFinished)
        {
            this.AddImage(uri, bitmap);
            this._runningImageTasks.Remove(uri);

            onFinished();
        }

        private void OnError(string uri, string error, Action onFinished)
        {
            this._errors.Add(uri, error);
            this._runningImageTasks.Remove(uri);

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
            if (!this._images.ContainsKey(uri))
            {
                this._images.Add(uri, imageModel);
            }
        }

        private class BitmapImageWithLastAccess
        {
            public BitmapSource Image { get; set; }

            public DateTime LastAccess { get; set; }
        }
    }
}
