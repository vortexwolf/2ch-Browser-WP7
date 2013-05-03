using System.Diagnostics;
using System.Windows.Controls;

using DvachBrowser.ViewModels;

namespace DvachBrowser.Views
{
    public partial class PostListView : UserControl
    {
        private bool _wasMouseUpCalled; // a temporary variable that is used in the OnTap event handler

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

        public void ScrollWithLayoutUpdate(object item)
        {
            if (item == null)
            {
                return;
            }

            this.list.UpdateLayout();
            this.list.ScrollIntoView(item);
        }

        private void OnBorderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!this._wasMouseUpCalled)
            {
                // if users clicked on links, don't select anything
                return;
            }

            // use a custom list box selection behavior
            var border = (Border)sender;
            var itemVm = (PostItemViewModel)border.DataContext;
            var listVm = (PostListViewModel)this.DataContext;
            
            // set the selected item manually
            if (listVm.SelectedPost == border.DataContext)
            {
                listVm.SelectedPost = null;
            }
            else
            {
                listVm.SelectedPost = itemVm;
            }
        }

        /// <summary>
        /// This method is called before Tap. It is not called if users clicked on links in RichTextBox.
        /// </summary>
        private void OnBorderMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this._wasMouseUpCalled = true;

            // cancel the default listbox selection
            e.Handled = true;
        }

        /// <summary>
        /// This method is called before MouseUp and before Tap.
        /// </summary>
        private void OnBorderManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            this._wasMouseUpCalled = false;
        }
    }
}
