using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DvachBrowser.Assets.Resources;

namespace DvachBrowser.Assets.HttpTasks
{
    public class HttpPostTask : HttpBaseTask
    {
        private readonly string _boundary = "----------" + DateTime.Now.Ticks.ToString();

        public HttpPostTask(string url, Dictionary<string, object> parameters, Action<string> onCompleted)
        {
            this.Url = url;
            this.Parameters = parameters;
            this.OnCompleted = onCompleted;
        }

        public string Url { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public Action<string> OnCompleted { get; private set; }

        public override async void Execute()
        {
            base.Execute();

            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(this.Url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", this._boundary);
            httpWebRequest.UserAgent = "2ch Browser (Windows Phone)";

            var task = httpWebRequest.GetRequestStreamAsync();
            await GetRequestStreamCallback(task, httpWebRequest);
        }

        private async Task GetRequestStreamCallback(Task<Stream> task, HttpWebRequest request)
        {
            Stream postStream = await task.ConfigureAwait(false);

            this.WriteMultipartObject(postStream, this.Parameters);
            postStream.Close();

            var responseTask = request.GetResponseAsync();
            await GetResponseCallback(responseTask);
        }

        private async Task GetResponseCallback(Task<WebResponse> task)
        {
            // get the response
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.StartsWith("[net_WebHeaderInvalidControlChars]"))
                {
                    // not an exception, everything is ok
                    this.InvokeInUiThread(() => this.OnCompleted(null));
                }
                else
                {
                    this.InvokeOnErrorHandler(ErrorMessages.HttpPostError);
                }

                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                this.InvokeOnErrorHandler((int)response.StatusCode + " " + response.StatusDescription);
                return;
            }

            // response stream
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string str = reader.ReadToEnd();
                    this.InvokeInUiThread(() => this.OnCompleted(str));
                }
            }
        }

        private void WriteMultipartObject(Stream stream, Dictionary<string, object> data)
        {
            StreamWriter writer = new StreamWriter(stream);
            if (data != null)
            {
                foreach (var entry in data)
                {
                    this.WriteEntry(writer, entry.Key, entry.Value);
                }
            }

            writer.Write("--");
            writer.Write(this._boundary);
            writer.WriteLine("--");
            writer.Flush();
        }

        private void WriteEntry(StreamWriter writer, string key, object value)
        {
            if (value != null)
            {
                writer.Write("--");
                writer.WriteLine(this._boundary);
                if (value is byte[])
                {
                    byte[] ba = value as byte[];

                    writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""; filename=""{1}""", key, "image.png");
                    writer.WriteLine(@"Content-Type: application/octet-stream");
                    writer.WriteLine(@"Content-Length: " + ba.Length);
                    writer.WriteLine();
                    writer.Flush();
                    Stream output = writer.BaseStream;

                    output.Write(ba, 0, ba.Length);
                    output.Flush();
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""", key);
                    writer.WriteLine();
                    writer.WriteLine(value.ToString());
                }
            }
        }
    }
}
