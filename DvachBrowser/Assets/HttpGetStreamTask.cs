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
using System.IO;

namespace DvachBrowser.Assets
{
    public class HttpGetStreamTask : HttpGetTask
    {
        public HttpGetStreamTask(string url, Action<MemoryStream> onPostExecute)
            : base(url)
        {
            this.OnPostExecute = onPostExecute;
        }

        protected override void OnStreamDownloaded(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            this.InvokeInUiThread(() => this.OnPostExecute(memoryStream));
        }

        public Action<MemoryStream> OnPostExecute { get; private set; }
    }
}
