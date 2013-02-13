using System.Runtime.Serialization;

namespace DvachBrowser.Models
{
    [DataContract]
    public class ThreadItemModel
    {
        [DataMember(Name = "image_count")]
        public int ImageCount { get; set; }

        [DataMember(Name = "reply_count")]
        public int ReplyCount { get; set; }

        [DataMember(Name = "posts")]
        public PostItemModel[][] Posts { get; set; }

        public PostItemModel OpenPost 
        {
            get { return this.Posts.Length > 0 ? this.Posts[0][0] : null; }
        }
    }
}
