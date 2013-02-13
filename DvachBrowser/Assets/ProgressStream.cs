using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DvachBrowser.Assets
{
    public class ProgressStream : Stream
    {
        private readonly Stream _stream;
        private long _length;

        public ProgressStream(Stream stream, long length)
        {
            this._stream = stream;
            this._length = length;
        }

        public override bool CanRead
        {
            get { return this._stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this._stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this._stream.CanWrite; }
        }

        public override void Flush()
        {
            this._stream.Flush();
        }

        public override long Length
        {
            get { return this._length; }
        }

        public override long Position
        {
            get
            {
                return this._stream.Position;
            }

            set
            {
                this._stream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readCount = this._stream.Read(buffer, offset, count);
            this.ReportProgress();

            return readCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long seekCount = this._stream.Seek(offset, origin);
            this.ReportProgress();

            return seekCount;
        }

        public override void SetLength(long value)
        {
            this._length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this._stream.Write(buffer, offset, count);
            this.ReportProgress();
        }

        public Action<double> OnProgressChanged { get; set; }

        private void ReportProgress()
        {
            if (this.OnProgressChanged != null)
            {
                var percent = this.Position / (double)this.Length;

                this.OnProgressChanged(percent);
            }
        }
    }
}
