using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace DvachBrowser.Assets.Controls
{
    public class HyperlinkProperties
    {
        public static Uri GetUri(DependencyObject obj)
        {
            return (Uri)obj.GetValue(UriProperty);
        }

        public static void SetUri(DependencyObject obj, Uri value)
        {
            obj.SetValue(UriProperty, value);
        }

        public static readonly DependencyProperty UriProperty =
            DependencyProperty.RegisterAttached("Uri", typeof(Uri), typeof(Hyperlink), new PropertyMetadata(null));
    }
}
