using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using DvachBrowser.ViewModels;

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
            this.list.UpdateLayout(); // important to prevent scrolling bugs
            this.list.ScrollIntoView(item);
        }

        private void OnBorderMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            if (this.list.SelectedItem == border.DataContext)
            {
                this.list.SelectedItem = null;
                e.Handled = true;
            }
        }
    }
}
