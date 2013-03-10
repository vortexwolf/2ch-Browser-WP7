using System;
using System.Linq;
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
using System.IO;
using System.Collections.Generic;

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
            if (_errors.ContainsKey(uri))
            {
                return new CachedBitmapImageModel() { IsCached = true, Error = _errors[uri] };
            }

            if (_images.ContainsKey(uri))
            {
                var imageModel = _images[uri];
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
            var httpGet = new HttpGetStreamTask(uri, stream => OnStreamDownloaded(uri, stream, onFinished));
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
                _errors.Add(uri, e.Message);
            }

            onFinished();
        }

        private void OnError(string uri, string error, Action onFinished)
        {
            _errors.Add(uri, error);

            onFinished();
        }

        private void AddImage(string uri, BitmapImage bitmap)
        {
            var imageModel = new BitmapImageWithLastAccess { Image = bitmap, LastAccess = DateTime.UtcNow };

            // remove old images if their number is too high
            if (_images.Count >= MaxImagesNumber)
            {
                var oldImagesUris = _images.OrderBy(b => b.Value.LastAccess).Take(RemoveExceedingImagesNumber).Select(b => b.Key).ToList();
                foreach (var imageUri in oldImagesUris)
                {
                    _images.Remove(imageUri);
                }
            }

            // add the image to cache
            _images.Add(uri, imageModel);
        }

        private class BitmapImageWithLastAccess
        {
            public BitmapImage Image { get; set; }

            public DateTime LastAccess { get; set; }
        }
    }
}
