using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.ViewModels
{
    public class AddPostViewModel : ValidationViewModel
    {
        public AddPostViewModel()
        {
            this._validator.AddValidationFor(() => this.Text).NotEmpty().Show("The comment cannot be empty.");
            this._validator.AddValidationFor(() => this.CaptchaAnswer).NotEmpty().Show("The captcha answer cannot be empty.");
        }

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
