﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

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
