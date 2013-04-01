using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets
{
    public static class Constants
    {
        public const string QueryStringBoard = "board";
        public const string QueryStringThread = "thread";
        public const string QueryStringUri = "uri";
        public const string QueryStringPage = "page";

        public const string ThreadListPageUri = "/Views/ThreadListPage.xaml";
        public const string PostListPageUri = "/Views/PostListPage.xaml";
        public const string BoardListPageUri = "/Views/BoardListPage.xaml";
        public const string ImageBrowserPageUri = "/Views/ImageBrowserPage.xaml";
        public const string AddPostPageUri = "/Views/AddPostPage.xaml";

        public const string DefaultBoardName = "b";
        public const int DefaultPage = 0;
        public const int FirstPage = 0;
        public const int LastPage = 19;

        public const string DvachHost = "2ch.hk";
    }
}
