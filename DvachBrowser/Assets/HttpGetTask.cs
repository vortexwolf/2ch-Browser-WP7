using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;

namespace DvachBrowser.Assets
{
    public abstract class HttpGetTask
    {
        public HttpGetTask(string url)
        {
            this.Url = url;
        }

        public void Execute()
        {
            // create the http request
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(this.Url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.UserAgent = "2ch Browser (Windows Phone)";

            // get the response asynchronously
            httpWebRequest.BeginGetResponse(this.OnGetResponseCompleted, httpWebRequest);
        }

        private void OnGetResponseCompleted(IAsyncResult ar)
        {
            var httpWebRequest = (HttpWebRequest)ar.AsyncState;

            // get the response
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)httpWebRequest.EndGetResponse(ar);
            }
            catch (WebException e)
            {
                this.InvokeOnErrorHandler("Unable to connect to the web page.");
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

            this.OnStreamDownloaded(stream);
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
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        public string Url { get; private set; }

        public Action<string> OnError { get; set; }

        public Action<double> OnProgressChanged { get; set; }
    }
}
