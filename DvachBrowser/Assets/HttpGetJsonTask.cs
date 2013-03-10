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
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace DvachBrowser.Assets
{
    public class HttpGetJsonTask<T> : HttpGetTask
    {
        public HttpGetJsonTask(string url, Action<T> onPostExecute) : base(url)
        {
            this.OnPostExecute = onPostExecute;
        }

        protected override void OnStreamDownloaded(Stream stream)
        {
            // deserialize json
            try
            {
                var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                var responseObject = (T)jsonSerializer.ReadObject(stream);

                this.InvokeInUiThread(() => this.OnPostExecute(responseObject));
            }
            catch (SerializationException e)
            {
                this.InvokeOnErrorHandler("Unable to read the response from the server.");
            }
        }

        public Action<T> OnPostExecute { get; private set; }
    }
}
