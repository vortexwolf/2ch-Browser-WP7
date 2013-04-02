using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using DvachBrowser.Assets.Controls;
using DvachBrowser.ViewModels;
using DvachBrowser.Views;

namespace DvachBrowser.Assets
{
    public class PopupDisplayer
    {
        public void ShowPost(PostItemViewModel post, Panel placeholder)
        {
            if (post == null)
            {
                return;
            }

            var view = new PostItemView() { DataContext = post };

            var popup = new PopupWindow(placeholder) { Content = view, IsContentScrollable = true };
            popup.Open();
        }

        public void ShowPosts(List<PostItemViewModel> posts, Panel placeholder)
        {
            if (posts.Count == 0)
            {
                return;
            }

            var view = new PostListView() { DataContext = posts };

            var popup = new PopupWindow(placeholder) { Content = view, IsContentScrollable = false };
            popup.Open();
        }
    }
}
