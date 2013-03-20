using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

using DvachBrowser.Assets.HttpTasks;
using DvachBrowser.Assets.Resources;

using Microsoft.Xna.Framework.Media;

namespace DvachBrowser.ViewModels
{
    public class ImageBrowserViewModel : LoadingBaseViewModel
    {
        public string Uri { get; set; }

        public void Init(string uri)
        {
            this.Uri = uri;
            this.Title = uri;
        }

        public void Download()
        {
            var task = new HttpGetStreamTask(this.Uri, this.OnStreamDownloaded);
            task.OnError = this.OnError;

            task.Execute();
        }

        private void OnStreamDownloaded(MemoryStream stream)
        {
            try
            {
                MediaLibrary library = new MediaLibrary();
                library.SavePicture(this.Uri, stream);

                MessageBox.Show(Strings.Notification_ImageSaved);
            }
            catch (Exception e)
            {
                this.OnError(e.Message);
            }
        }

        private void OnError(string message)
        {
            MessageBox.Show(message);
        }
    }
}
