using System.Runtime.Serialization;

namespace DvachBrowser.Models
{
    [DataContract]
    public class ThreadListModel
    {
        [DataMember(Name = "threads")]
        public ThreadItemModel[] Threads { get; set; }
    }
}
