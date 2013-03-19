using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.ViewModels
{
    public class LoadingBaseViewModel : ViewModel
    {
        private string _title;

        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.OnPropertyChanged("Title");
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

        private double _progress;

        public double Progress
        {
            get { return this._progress; }
            set
            {
                this._progress = value;
                this.OnPropertyChanged("Progress");
            }
        }

        private bool _isError;

        public bool IsError
        {
            get { return this._isError; }
            set
            {
                this._isError = value;
                this.OnPropertyChanged("IsError");
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return this._errorMessage; }
            set
            {
                this._errorMessage = value;
                this.OnPropertyChanged("ErrorMessage");
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

        public virtual void ShowLoading()
        {
            this.IsLoading = true;
            this.IsError = false;
            this.IsLoaded = false;
        }

        public virtual void HideLoading()
        {
            this.IsLoading = false;
            this.IsError = false;
            this.IsLoaded = true;
        }

        public virtual void ShowError(string message)
        {
            this.IsLoading = false;
            this.IsError = true;
            this.ErrorMessage = message;
            this.IsLoaded = false;
        }

        protected virtual void UpdateProgress(double value)
        {
            this.Progress = value;
        }
    }
}
