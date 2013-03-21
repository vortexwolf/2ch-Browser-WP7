using System;
using System.IO;
using System.Text;

using DvachBrowser.Assets.Resources;

namespace DvachBrowser.Assets.HttpTasks
{
    public class HttpGetStringTask : HttpGetTask
    {
        public HttpGetStringTask(string url, Action<string> onPostExecute) : base(url)
        {
            this.OnPostExecute = onPostExecute;
        }

        public Action<string> OnPostExecute { get; private set; }

        protected override void OnStreamDownloaded(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                this.ParseString(memoryStream);
            }
        }

        private void ParseString(MemoryStream stream)
        {
            try
            {
                string str = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);

                this.InvokeInUiThread(() => this.OnPostExecute(str));
            }
            catch (Exception e)
            {
                this.InvokeOnErrorHandler(ErrorMessages.DownloadError);
            }
        }
    }
}
