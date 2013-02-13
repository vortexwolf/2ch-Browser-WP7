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
    public class ThreadListViewModel : INotifyPropertyChanged
    {
        public ThreadListViewModel()
        {
            this.Threads = new ObservableCollection<ThreadItemViewModel>();

            // load threads from the network
            var httpGet = new HttpGetTask<ThreadListModel>("http://2ch.hk/test/wakaba.json", this.OnPostLoadingThreads);
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
            Debug.WriteLine(value);
            this.Progress = value;
        }
        
        private void DisplayThreads(ThreadListModel threadList)
        {
            foreach (var thread in threadList.Threads)
            {
                var vm = new ThreadItemViewModel(thread);

                this.Threads.Add(vm);
            }
        }

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
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
