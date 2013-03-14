using System;
using System.Collections.ObjectModel;
using DvachBrowser.Assets;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadListViewModel : ViewModel
    {
        private readonly BitmapManager _bitmapManager;

        private HttpGetJsonTask<ThreadListModel> _currentTask;

        public ThreadListViewModel()
        {
            this.Threads = new ObservableCollection<ThreadItemViewModel>();
            this._bitmapManager = Container.Resolve<BitmapManager>();
        }

        public void Load(string boardName)
        {
            if (this._currentTask != null)
            {
                this._currentTask.Cancel();
            }

            this.BoardName = boardName;
            this.Title = "/" + boardName + "/";

            // load threads from the network
            string threadsUrl = string.Format("http://2ch.hk/{0}/wakaba.json?nocache={1}", boardName, DateTime.UtcNow);
            this._currentTask = new HttpGetJsonTask<ThreadListModel>(threadsUrl, this.OnPostLoadingThreads);
            this._currentTask.OnError = this.OnError;
            this._currentTask.OnProgressChanged = this.OnProgressChanged;

            this.OnPreLoadingThreads();
            this._currentTask.Execute();
        }

        public void Refresh()
        {
            this.Load(this.BoardName);
        }

        private void OnPreLoadingThreads()
        {
            this.IsLoading = true;
            this.IsError = false;
            this.IsListLoaded = false;
        }

        private void OnPostLoadingThreads(ThreadListModel responseObject)
        {
            this.DisplayThreads(responseObject);
            this.IsLoading = false;
            this.IsError = false;
            this.IsListLoaded = true;

            this._currentTask = null;
        }

        private void OnError(string message)
        {
            this.IsLoading = false;
            this.IsError = true;
            this.ErrorMessage = message;
            this.IsListLoaded = false;

            this._currentTask = null;
        }

        private void OnProgressChanged(double value)
        {
            this.Progress = value;
        }
        
        private void DisplayThreads(ThreadListModel threadList)
        {
            this.Threads.Clear();

            foreach (var thread in threadList.Threads)
            {
                var vm = new ThreadItemViewModel(this.BoardName, thread, this._bitmapManager);

                this.Threads.Add(vm);
            }
        }

        public string BoardName { get; set; }

        public ObservableCollection<ThreadItemViewModel> Threads { get; set; }

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

        private bool _isListLoaded;

        public bool IsListLoaded
        {
            get { return this._isListLoaded; }
            set 
            {
                this._isListLoaded = value;
                this.OnPropertyChanged("IsListLoaded");
            }
        }
    }
}
