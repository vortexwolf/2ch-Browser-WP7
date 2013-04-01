using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

using DvachBrowser.Assets.Resources;

namespace DvachBrowser.Assets.HttpTasks
{
    public abstract class HttpGetTask : HttpBaseTask
    {
        protected HttpGetTask(string url)
        {
            this.Url = url;
        }

        public override void Execute()
        {
            base.Execute();

            // create the http request
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(this.Url);
            httpWebRequest.Method = "GET";
            httpWebRequest.UserAgent = "2ch Browser (Windows Phone)";

            // get the response asynchronously
            httpWebRequest.BeginGetResponse(this.OnGetResponseCompleted, httpWebRequest);
        }

        protected abstract void OnStreamDownloaded(Stream stream);

        private void OnGetResponseCompleted(IAsyncResult ar)
        {
            if (this._isCancelled)
            {
                return;
            }

            var httpWebRequest = (HttpWebRequest)ar.AsyncState;

            // get the response
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)httpWebRequest.EndGetResponse(ar);
            }
            catch (Exception e)
            {
                this.InvokeOnErrorHandler(ErrorMessages.WebPageLoadError);
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                this.InvokeOnErrorHandler((int)response.StatusCode + " " + response.StatusDescription);
                return;
            }

            // response stream
            Stream stream;
            if (this.OnProgressChanged != null && response.Headers.AllKeys.Any(key => key == "Content-Length"))
            {
                var progressStream = new ProgressStream(response.GetResponseStream(), response.ContentLength);
                progressStream.OnProgressChanged = v => this.InvokeInUiThread(() => this.OnProgressChanged(v));
                stream = progressStream;
            }
            else
            {
                stream = response.GetResponseStream();
            }

            if (!this._isCancelled)
            {
                this.OnStreamDownloaded(stream);
            }
        }

        public string Url { get; private set; }

        public Action<double> OnProgressChanged { get; set; }
    }
}
