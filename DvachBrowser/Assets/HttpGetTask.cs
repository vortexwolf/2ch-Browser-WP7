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
using System.Runtime.Serialization.Json;

namespace DvachBrowser.Assets
{
    public class HttpGetTask<T>
    {
        public HttpGetTask(string url, Action<T> onPostExecute)
        {
            this.Url = url;
            this.OnPostExecute = onPostExecute;
        }

        public void Execute()
        {
            // create the http request
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(this.Url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";

            // get the response asynchronously
            httpWebRequest.BeginGetResponse(OnGetResponseCompleted, httpWebRequest);
        }

        private void OnGetResponseCompleted(IAsyncResult ar)
        {
            var httpWebRequest = (HttpWebRequest)ar.AsyncState;

            // get the response
            var response = (HttpWebResponse)httpWebRequest.EndGetResponse(ar);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (this.OnError != null)
                {
                    this.InvokeInUiThread(() => this.OnError(response.StatusCode, response.StatusDescription));
                }

                return;
            }

            // response stream
            var progressStream = new ProgressStream(response.GetResponseStream(), response.ContentLength);
            progressStream.OnProgressChanged = v => this.InvokeInUiThread(() => this.OnProgressChanged(v));

            // deserialize json
            var jsonSerializer = new DataContractJsonSerializer(typeof(T));
            var responseObject = (T)jsonSerializer.ReadObject(progressStream);

            // call the virtual method
            this.InvokeInUiThread(() => this.OnPostExecute(responseObject));
        }

        private void InvokeInUiThread(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        public string Url { get; private set; }

        public Action<T> OnPostExecute { get; private set; }

        public Action<HttpStatusCode, string> OnError { get; set; }

        public Action<double> OnProgressChanged { get; set; }
    }

}
