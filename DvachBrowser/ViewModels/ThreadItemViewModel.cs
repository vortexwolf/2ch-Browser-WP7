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
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadItemViewModel
    {
        public ThreadItemViewModel(ThreadItemModel thread)
        {
            this.MapModel(thread);
        }

        public void MapModel(ThreadItemModel thread)
        {
            this.Subject = thread.OpenPost.Subject;
            this.RepliesImagesMessage = string.Format("{0} posts and {1} images", thread.ReplyCount, thread.ImageCount);
            this.Comment = thread.OpenPost.Comment;

            if (this.Comment.Length > 200)
            {
                this.Comment = this.Comment.Substring(0, 200) + "...";
            }
        }

        public string Subject { get; set; }

        public string Comment { get; set; }

        public string RepliesImagesMessage { get; set; }
    }
}
