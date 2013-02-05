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
using System.Runtime.Serialization;

namespace DvachBrowser.Models
{
    [DataContract]
    public class PostItemModel
    {
        [DataMember(Name = "num")]
        public int Number { get; set; }

        [DataMember(Name = "timestamp")]
        public int Timestamp { get; set; }

        [DataMember(Name = "thumbnail")]
        public string ThumbnailUri { get; set; }

        [DataMember(Name = "image")]
        public string ImageUri { get; set; }

        [DataMember(Name = "size")]
        public int ImageSize { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }
    }
}
