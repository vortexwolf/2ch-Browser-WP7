using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

using DvachBrowser.Assets.Resources;

namespace DvachBrowser.Assets
{
    public abstract class HttpGetTask
    {
        private bool _isCancelled;
        private bool _isExecuted;

        protected HttpGetTask(string url)
        {
            this.Url = url;
        }

        public void Execute()
        {
            if (this._isExecuted)
            {
                throw new NotSupportedException();
            }

            this._isExecuted = true;

            // create the http request
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(this.Url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.UserAgent = "2ch Browser (Windows Phone)";

            // get the response asynchronously
            httpWebRequest.BeginGetResponse(this.OnGetResponseCompleted, httpWebRequest);
        }

        public void Cancel()
        {
            this._isCancelled = true;
        }

        protected abstract void OnStreamDownloaded(Stream stream);

        protected void InvokeOnErrorHandler(string message)
        {
            if (this.OnError != null)
            {
                this.InvokeInUiThread(() => this.OnError(message));
            }
        }

        protected void InvokeInUiThread(Action action)
        {
            if (!this._isCancelled)
            {
                Deployment.Current.Dispatcher.BeginInvoke(action);
            }
        }

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
            catch (WebException e)
            {
                this.InvokeOnErrorHandler(ErrorMessages.WebPageLoadError);
                return;
            }
            catch (Exception e)
            {
                this.InvokeOnErrorHandler(e.Message);
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

        public Action<string> OnError { get; set; }

        public Action<double> OnProgressChanged { get; set; }
    }
}
