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
    public class PostItemViewModel
    {
        public const int MaxCharNumber = 160;

        public PostItemViewModel(PostItemModel thread)
        {
            this.MapModel(thread);
        }

        public void MapModel(PostItemModel post)
        {
            this.Number = post.Number;
            this.Subject = post.Subject;
            this.Comment = post.Comment;

            if (this.Comment.Length > MaxCharNumber)
            {
                this.Comment = this.Comment.Substring(0, MaxCharNumber) + "...";
            }
        }

        public string Number { get; set; }

        public string Subject { get; set; }

        public string Comment { get; set; }
    }
}
