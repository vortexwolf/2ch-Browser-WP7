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
using DvachBrowser.Assets;
using DvachBrowser.Models;
using System.Collections.ObjectModel;

namespace DvachBrowser.ViewModels
{
    public class PostListViewModel : ViewModel
    {
        public PostListViewModel()
        {
            this.Posts = new ObservableCollection<PostItemViewModel>();
        }

        public void Load(string boardName, string threadNumber)
        {
            this.BoardName = boardName;
            this.ThreadNumber = threadNumber;
            this.Title = boardName + "/" + threadNumber;

            // load posts from the network
            string postsUrl = string.Format("http://2ch.hk/{0}/res/{1}.json", boardName, threadNumber);
            var httpGet = new HttpGetTask<PostListModel>(postsUrl, this.OnPostLoadingPosts);
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

        private void OnPostLoadingPosts(PostListModel responseObject)
        {
            this.DisplayPosts(responseObject);
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

        private void DisplayPosts(PostListModel postList)
        {
            foreach (var postArray in postList.Posts)
            {
                var post = postArray[0];
                var vm = new PostItemViewModel(post);

                this.Posts.Add(vm);
            }
        }
        
        public ObservableCollection<PostItemViewModel> Posts { get; set; }

        private string _boardName;

        public string BoardName
        {
            get { return _boardName; }
            set
            {
                _boardName = value;
                OnPropertyChanged("BoardName");
            }
        }


        private string _threadNumber;

        public string ThreadNumber
        {
            get { return _threadNumber; }
            set
            {
                _threadNumber = value;
                OnPropertyChanged("ThreadNumber");
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
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
    }
}
