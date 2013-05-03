using System.Runtime.Serialization;

namespace DvachBrowser.Models
{
    [DataContract]
    public class PostItemModel
    {
        [DataMember(Name = "num")]
        public long Number { get; set; }

        [DataMember(Name = "timestamp")]
        public long Timestamp { get; set; }

        [DataMember(Name = "thumbnail")]
        public string ThumbnailUri { get; set; }

        [DataMember(Name = "image")]
        public string ImageUri { get; set; }

        [DataMember(Name = "video")]
        public string VideoHtml { get; set; }

        [DataMember(Name = "size")]
        public int ImageSize { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }
    }
}
