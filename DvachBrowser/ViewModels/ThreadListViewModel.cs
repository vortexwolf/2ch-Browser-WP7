using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DvachBrowser.Models;
using DvachBrowser.Assets;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        }

        private void OnPostLoadingThreads(ThreadListModel responseObject)
        {
            this.DisplayThreads(responseObject);
            this.IsLoading = false;
        }

        private void OnError(HttpStatusCode code, string description)
        {
            this.IsLoading = false;
            MessageBox.Show((int)code + description);
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
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        private double _progress;

        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
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
