using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using DvachBrowser.Assets;
using DvachBrowser.Assets.HttpTasks;
using DvachBrowser.Assets.Resources;

namespace DvachBrowser.ViewModels
{
    public class AddPostViewModel : ValidationViewModel
    {
        private readonly PageNavigationService _navigationService;
        private readonly PostResponseParser _postResponseParser;

        private HttpPostTask _currentPostTask;

        public AddPostViewModel()
        {
            this._navigationService = Container.Resolve<PageNavigationService>();
            this._postResponseParser = new PostResponseParser();

            this.CaptchaModel = new CaptchaViewModel();
            this.IsLoaded = true;

            this._validator.AddValidationFor(() => this.Text).NotEmpty().Show(Strings.Validation_Comment);
            this._validator.AddValidationFor(() => this.CaptchaAnswer).NotEmpty().Show(Strings.Validation_CaptchaAnswer);
        }

        public void Init(string boardName, string threadNumber)
        {
            this.BoardName = boardName;
            this.ThreadNumber = threadNumber;

            this.CaptchaModel.RefreshImage();
        }
        
        public string BoardName { get; private set; }

        public string ThreadNumber { get; private set; }

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

        private bool _isLoading;

        public bool IsLoading
        {
            get { return this._isLoading; }
            set
            {
                this._isLoading = value;
                this.OnPropertyChanged("IsLoading");
            }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get { return this._isLoaded; }
            set
            {
                this._isLoaded = value;
                this.OnPropertyChanged("IsLoaded");
            }
        }

        public void Send()
        {
            if (this._currentPostTask != null)
            {
                return; // don't post the 2nd time
            }

            if (!this.ValidateAll())
            {
                return;
            }

            string url = string.Format("http://2ch.hk/{0}/wakaba.pl", this.BoardName);
            var parameters = this.CreatePostParameters();

            this._currentPostTask = new HttpPostTask(url, parameters, this.OnPostCompleted);
            this._currentPostTask.OnError = this.OnPostError;

            this.ShowLoading();
            this._currentPostTask.Execute();
        }

        private Dictionary<string, object> CreatePostParameters()
        {
            var entity = new Dictionary<string, object>();
            entity.Add("task", "роst");
            entity.Add("parent", this.ThreadNumber);
            entity.Add("captcha", this.CaptchaModel.Key); // captcha key
            entity.Add("captcha_value_id_06", this.CaptchaAnswer); // captcha answer
            entity.Add("shampoo", this.Text); // comment
            if (this.IsSage)
            {
                entity.Add("nabiki", "sage"); // email
            }

            return entity;
        }

        private void OnPostCompleted(string response)
        {
            this.HideLoading();

            var postResult = this._postResponseParser.ParsePostResponse(response);

            if (postResult.IsSuccess)
            {
                this._navigationService.GoBack();
            }
            else
            {
                this.OnPostError(postResult.ErrorMessage);
            }
        }

        private void OnPostError(string message)
        {
            this.HideLoading();

            if (message == "Ошибка: Неверный код подтверждения.")
            {
                this.CaptchaModel.RefreshImage();
            }

            MessageBox.Show(message);
        }

        private void ShowLoading()
        {
            this.IsLoading = true;
            this.IsLoaded = false;
        }

        private void HideLoading()
        {
            this.IsLoading = false;
            this.IsLoaded = true;
            this._currentPostTask = null;
        }
    }
}
