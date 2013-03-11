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

using DvachBrowser.Assets;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class PostItemViewModel : ThreadPostBaseViewModel
    {
        public PostItemViewModel(string boardName, BitmapManager bitmapManager) : base(boardName, bitmapManager)
        {
        }
    }
}
