using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DvachBrowser.Assets.Resources;

namespace DvachBrowser.ViewModels
{
    public class AddPostViewModel : ValidationViewModel
    {
        public AddPostViewModel()
        {
            this._validator.AddValidationFor(() => this.Text).NotEmpty().Show(Strings.Validation_Comment);
            this._validator.AddValidationFor(() => this.CaptchaAnswer).NotEmpty().Show(Strings.Validation_CaptchaAnswer);

            this.CaptchaModel = new CaptchaViewModel();
            this.CaptchaModel.RefreshImage();
        }

        public CaptchaViewModel CaptchaModel { get; set; }

        public bool IsSage { get; set; }

        private string _text;

        public string Text
        {
            get { return this._text; }
            set
            {
                this._text = value;
                this.RaisePropertyChanged("Text");
            }
        }

        private string _captchaAnswer;

        public string CaptchaAnswer
        {
            get { return this._captchaAnswer; }
            set
            {
                this._captchaAnswer = value;
                this.RaisePropertyChanged("CaptchaAnswer");
            }
        }

        public void Send()
        {
            if (!this.ValidateAll())
            {
                return;
            }

            // else send
        }
    }
}
