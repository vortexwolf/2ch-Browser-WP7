using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DvachBrowser.Assets.HttpTasks
{
    public class HttpGetImageTask : HttpGetTask
    {
        public HttpGetImageTask(string url, Action<BitmapSource> onPostExecute)
            : base(url)
        {
            this.OnPostExecute = onPostExecute;
        }

        public Action<BitmapSource> OnPostExecute { get; private set; }

        protected override void OnStreamDownloaded(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            this.InvokeInUiThread(() => this.ParseBitmap(this.Url, memoryStream));
        }

        private void ParseBitmap(string uri, MemoryStream stream)
        {
            BitmapSource bitmap = null;
            try
            {
                // the special case for gif images
                bitmap = this.DecodeGif(stream);

                // try to use the default way for all other images and for failed gif images
                if (bitmap == null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                }

                this.InvokeInUiThread(() => this.OnPostExecute(bitmap));
            }
            catch (Exception e)
            {
                this.InvokeOnErrorHandler(e.Message);
            }
        }

        private WriteableBitmap DecodeGif(MemoryStream stream)
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
    }
}
