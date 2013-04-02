using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using DvachBrowser.Assets.Resources;

using Microsoft.Phone.Controls;

namespace DvachBrowser.Assets.Controls
{
    public class PopupWindow
    {
        private readonly Panel _placeholder;
        private Border _content;

        public PopupWindow(Panel placeholder)
        {
            this._placeholder = placeholder;
        }

        public bool IsContentScrollable { get; set; }

        public UIElement Content { get; set; }

        public void Open()
        {
            this._content = this.CreatePopupContent();
            this._placeholder.Children.Add(this._content);

            // transparent background that ignores click events, similar to IsHitTestVisible=false
            if (this._placeholder.Background == null)
            {
                this._placeholder.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        public void Close()
        {
            this._placeholder.Children.Remove(this._content);

            // remove background if the placeholder has no popups, so now IsHitTestVisible=true
            if (this._placeholder.Children.Count == 0)
            {
                this._placeholder.Background = null;
            }
        }

        private Border CreatePopupContent()
        {
            // border
            var border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Gray);
            border.BorderThickness = new Thickness(2);
            border.Background = (Brush)App.Current.Resources["ThemeBackground"];
            border.VerticalAlignment = VerticalAlignment.Center;
            border.Width = ((App)App.Current).RootFrame.ActualWidth;
            border.MaxHeight = ((App)App.Current).RootFrame.ActualHeight - 150;

            // grid
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // button
            var button = new Button() { Content = Strings.Ok, HorizontalAlignment = HorizontalAlignment.Center };
            button.SetValue(Grid.RowProperty, 1);
            button.Click += (s, e) => this.Close();

            grid.Children.Add(button);
            this.AddContentToTheGrid(grid);

            border.Child = grid;

            return border;
        }

        private void AddContentToTheGrid(Grid grid)
        {
            UIElement content;

            if (!this.IsContentScrollable)
            {
                content = this.Content;
            }
            else
            {
                content = new ScrollViewer() { Content = this.Content };
            }

            content.SetValue(Grid.RowProperty, 0);
            grid.Children.Add(content);
        }
    }
}
