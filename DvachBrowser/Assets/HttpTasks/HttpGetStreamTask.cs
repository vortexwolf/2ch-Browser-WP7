using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets.HttpTasks
{
    public class HttpGetStreamTask : HttpGetTask
    {
        public HttpGetStreamTask(string url, Action<MemoryStream> onPostExecute) : base(url)
        {
            this.OnPostExecute = onPostExecute;
        }

        public Action<MemoryStream> OnPostExecute { get; private set; }

        protected override void OnStreamDownloaded(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            this.InvokeInUiThread(() => this.OnPostExecute(memoryStream));
        }
    }
}
