using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Resources;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadItemViewModel : ThreadPostBaseViewModel
    {
        public const int MaxCharNumber = 160;

        public ThreadItemViewModel(string boardName, ThreadItemModel thread, BitmapManager bitmapManager) : base(boardName, bitmapManager)
        {
            this.MapModel(thread);
        }

        public void MapModel(ThreadItemModel thread)
        {
            base.MapModel(thread.OpenPost);

            this.RepliesImagesMessage = string.Format(Strings.DataFormat_PostsAndImages, thread.ReplyCount, thread.ImageCount);
        }

        public string RepliesImagesMessage { get; private set; }
    }
}
