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
            get { return Posts.Length > 0 ? Posts[0][0] : null; }
        }
    }
}
