using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using DvachBrowser.Assets;
using DvachBrowser.Assets.HttpTasks;
using DvachBrowser.Assets.Resources;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using System.IO;

namespace DvachBrowser.ViewModels
{
    public class AddPostViewModel : ValidationViewModel
    {
        private readonly PageNavigationService _navigationService;
        private readonly PostResponseParser _postResponseParser;
        private readonly DvachUrlBuilder _urlBuilder;

        private HttpPostTask _currentPostTask;

        public AddPostViewModel()
        {
            this._navigationService = Container.Resolve<PageNavigationService>();
            this._postResponseParser = Container.Resolve<PostResponseParser>();
            this._urlBuilder = Container.Resolve<DvachUrlBuilder>();

            this.CaptchaModel = new CaptchaViewModel();
            this.IsLoaded = true;
            
            this.AttachFileCommand = new RelayCommand(this.AttachFile);
            this.RemoveFileCommand = new RelayCommand(this.RemoveFile);

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

        public byte[] AttachedFileBytes { get; set; }
        
        public ICommand AttachFileCommand { get; set; }

        public ICommand RemoveFileCommand { get; set; }
        
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

        private bool _hasAttachment;

        public bool HasAttachment
        {
            get { return this._hasAttachment; }
            set
            {
                this._hasAttachment = value;
                this.OnPropertyChanged("HasAttachment");
            }
        }

        private string _attachmentName;

        public string AttachmentName
        {
            get { return this._attachmentName; }
            set
            {
                this._attachmentName = value;
                this.OnPropertyChanged("AttachmentName");
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

            string url = this._urlBuilder.BuildAddPostUrl(this.BoardName);
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

            if (this.HasAttachment)
            {
                entity.Add("file", this.AttachedFileBytes);
            }

            return entity;
        }

        private void OnPostCompleted(string response)
        {
            this.HideLoading();

            var postResult = this._postResponseParser.ParsePostResponse(response);

            if (postResult.IsSuccess)
            {
                Container.Resolve<AddPostStorage>().IsSentSuccessfully = true;
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

        private void AttachFile()
        {
            var chooseTask = new PhotoChooserTask();
            chooseTask.ShowCamera = true;
            chooseTask.Completed += (s, e) => this.OnChoosePhotoCompleted(e);
            chooseTask.Show();
        }

        private void OnChoosePhotoCompleted(PhotoResult result)
        {
            if (result.TaskResult == TaskResult.OK)
            {
                this.HasAttachment = true;
                this.AttachmentName = Path.GetFileName(result.OriginalFileName);

                using (MemoryStream fileBytes = new MemoryStream())
                {
                    result.ChosenPhoto.CopyTo(fileBytes);
                    this.AttachedFileBytes = fileBytes.GetBuffer();
                }
            }
        }

        private void RemoveFile()
        {
            this.HasAttachment = false;
            this.AttachmentName = null;
            this.AttachedFileBytes = null;
        }
    }
}
