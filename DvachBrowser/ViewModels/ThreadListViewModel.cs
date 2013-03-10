using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DvachBrowser.Assets;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadListViewModel : ViewModel
    {
        private readonly BitmapManager _bitmapManager;

        public ThreadListViewModel()
        {
            this.Threads = new ObservableCollection<ThreadItemViewModel>();
            this._bitmapManager = Container.Resolve<BitmapManager>();

            this.Load("test");
        }

        public void Load(string boardName)
        {
            this.BoardName = boardName;
            this.Title = boardName;

            // load threads from the network
            var httpGet = new HttpGetJsonTask<ThreadListModel>("http://2ch.hk/test/wakaba.json", this.OnPostLoadingThreads);
            httpGet.OnError = this.OnError;
            httpGet.OnProgressChanged = this.OnProgressChanged;

            this.OnPreLoadingThreads();
            httpGet.Execute();
        }

        private void OnPreLoadingThreads()
        {
            this.IsLoading = true;
            this.IsError = false;
        }

        private void OnPostLoadingThreads(ThreadListModel responseObject)
        {
            this.DisplayThreads(responseObject);
            this.IsLoading = false;
            this.IsError = false;
        }

        private void OnError(string message)
        {
            this.IsLoading = false;
            this.IsError = true;
            this.ErrorMessage = message;
        }

        private void OnProgressChanged(double value)
        {
            this.Progress = value;
        }
        
        private void DisplayThreads(ThreadListModel threadList)
        {
            foreach (var thread in threadList.Threads)
            {
                var vm = new ThreadItemViewModel(this.BoardName, thread, this._bitmapManager);

                this.Threads.Add(vm);
            }
        }

        public string BoardName { get; set; }

        public string Title { get; set; }

        public ObservableCollection<ThreadItemViewModel> Threads { get; set; }
        
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
    }
}
