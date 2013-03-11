using System.Runtime.Serialization;

namespace DvachBrowser.Models
{
    [DataContract]
    public class PostListModel
    {
        [DataMember(Name = "thread")]
        public PostItemModel[][] Posts { get; set; }
    }
}
