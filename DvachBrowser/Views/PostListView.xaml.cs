using System.Windows.Controls;

namespace DvachBrowser.Views
{
    public partial class PostListView : UserControl
    {
        public PostListView()
        {
            this.InitializeComponent();
        }

        public void ScrollToTop()
        {
            if (this.list.Items.Count > 0)
            {
                this.ScrollWithLayoutUpdate(this.list.Items[0]);
            }
        }

        public void ScrollToBottom()
        {
            if (this.list.Items.Count > 0)
            {
                this.ScrollWithLayoutUpdate(this.list.Items[this.list.Items.Count - 1]);
            }
        }

        private void ScrollWithLayoutUpdate(object item)
        {
            this.list.ScrollIntoView(item);
            this.list.UpdateLayout();
            this.list.ScrollIntoView(item);
        }
    }
}
