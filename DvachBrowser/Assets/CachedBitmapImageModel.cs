using System.Windows.Media.Imaging;

namespace DvachBrowser.Assets
{
    public class CachedBitmapImageModel
    {
        public bool IsCached { get; set; }

        public string Error { get; set; }

        public BitmapSource Image { get; set; }
    }
}
