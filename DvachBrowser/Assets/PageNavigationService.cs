using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.Phone.Controls;

namespace DvachBrowser.Assets
{
    public class PageNavigationService
    {
        public void Navigate(string uri)
        {
            RootFrame.Navigate(new Uri(uri, UriKind.Relative));
        }

        public void GoBack()
        {
            RootFrame.GoBack();
        }

        public void NavigateToThreadListPage(string boardName, int? page)
        {
            string queryString = new QueryStringBuilder()
                .Add(Constants.QueryStringBoard, boardName)
                .Add(Constants.QueryStringPage, page != null ? page.ToString() : null)
                .Build();

            this.Navigate(Constants.ThreadListPageUri + queryString);
        }

        public void NavigateToPostListPage(string boardName, string pageNumber)
        {
            string queryString = new QueryStringBuilder()
                .Add(Constants.QueryStringBoard, boardName)
                .Add(Constants.QueryStringThread, pageNumber)
                .Build();

            this.Navigate(Constants.PostListPageUri + queryString);
        }

        public void NavigateToAddPostPage(string boardName, string threadNumber, string postNumber)
        {
            string queryString = new QueryStringBuilder()
                .Add(Constants.QueryStringBoard, boardName)
                .Add(Constants.QueryStringThread, threadNumber)
                .Add(Constants.QueryStringPost, postNumber)
                .Build();

            this.Navigate(Constants.AddPostPageUri + queryString);
        }

        private static PhoneApplicationFrame RootFrame
        {
            get { return ((App)Application.Current).RootFrame; }
        }
    }
}
