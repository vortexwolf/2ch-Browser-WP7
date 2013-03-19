using System;
using System.Windows.Media.Imaging;

using DvachBrowser.Assets;

using GalaSoft.MvvmLight.Command;

namespace DvachBrowser.ViewModels
{
    public class CaptchaViewModel : LoadingBaseViewModel
    {
        private HttpGetStringTask _currentStringTask;
        private HttpGetImageTask _currentImageTask;

        public CaptchaViewModel()
        {
            this.RefreshImageCommand = new RelayCommand(this.RefreshImage);
        }

        public RelayCommand RefreshImageCommand { get; private set; }

        private BitmapSource _image;

        public BitmapSource Image
        {
            get { return this._image; }
            set
            {
                this._image = value;
                this.RaisePropertyChanged("Image");
            }
        }

        public void RefreshImage()
        {
            if (this._currentStringTask != null)
            {
                this._currentStringTask.Cancel();
            }

            if (this._currentImageTask != null)
            {
                this._currentImageTask.Cancel();
            }

            string checkUri = string.Format("http://2ch.hk/makaba/captcha.fcgi?nocache={0}", DateTime.UtcNow);
            this._currentStringTask = new HttpGetStringTask(checkUri, this.OnCheckStringLoaded);
            this._currentStringTask.OnError = this.ShowError;

            this.ShowLoading();
            this._currentStringTask.Execute();
        }

        public override void ShowError(string message)
        {
            base.ShowError(message);

            this._currentImageTask = null;
            this._currentStringTask = null;
        }

        private void OnCheckStringLoaded(string str)
        {
            string key = str.Substring(str.IndexOf('\n') + 1);

            this.LoadCaptcha(key);

            this._currentStringTask = null;
        }
        
        private void LoadCaptcha(string key)
        {
            string imageUrl = string.Format("http://i.captcha.yandex.net/image?key={0}", key);

            this._currentImageTask = new HttpGetImageTask(imageUrl, this.OnImageDownloaded);
            this._currentImageTask.OnError = this.ShowError;

            this._currentImageTask.Execute();
        }

        private void OnImageDownloaded(BitmapSource bitmap)
        {
            this.DisplayImage(bitmap);

            this.HideLoading();

            this._currentImageTask = null;
        }

        private void DisplayImage(BitmapSource bitmap)
        {
            this.Image = bitmap;
        }
    }
}
