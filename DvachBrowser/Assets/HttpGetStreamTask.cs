using System;
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
