using System;
using System.Collections.ObjectModel;
using System.Linq;

using DvachBrowser.Assets;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class PostListViewModel : ViewModel
    {
        private readonly BitmapManager _bitmapManager;

        private HttpGetJsonTask<PostListModel> _currentTask;

        public PostListViewModel()
        {
            this.Posts = new ObservableCollection<PostItemViewModel>();
            this._bitmapManager = Container.Resolve<BitmapManager>();
        }

        public void Load(string boardName, string threadNumber)
        {
            this.IsLoadCalled = true;

            if (this._currentTask != null)
            {
                this._currentTask.Cancel();
            }

            this.BoardName = boardName;
            this.ThreadNumber = threadNumber;
            this.Title = "/" + boardName + "/" + threadNumber;

            // load posts from the network
            string postsUrl = string.Format("http://2ch.hk/{0}/res/{1}.json?nocache={2}", boardName, threadNumber, DateTime.UtcNow);
            this._currentTask = new HttpGetJsonTask<PostListModel>(postsUrl, this.OnPostLoadingPosts);
            this._currentTask.OnError = this.OnError;
            this._currentTask.OnProgressChanged = this.OnProgressChanged;

            this.OnPreLoadingThreads();
            this._currentTask.Execute();
        }

        public void Refresh()
        {
            this.Load(this.BoardName, this.ThreadNumber);
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
            this._currentTask = null;
        }

        private void OnError(string message)
        {
            this.IsLoading = false;
            this.IsError = true;
            this.ErrorMessage = message;
            this._currentTask = null;
        }

        private void OnProgressChanged(double value)
        {
            this.Progress = value;
        }

        private void DisplayPosts(PostListModel postList)
        {
            var lastPostNumber = this.Posts.Select(p => p.Number).DefaultIfEmpty(0).Max();
            var newPosts = postList.Posts.Select(postArray => postArray[0]).SkipWhile(post => post.Number <= lastPostNumber);

            foreach (var post in newPosts)
            {
                var vm = new PostItemViewModel(this.BoardName, this._bitmapManager);
                vm.MapModel(post);

                this.Posts.Add(vm);
            }
        }

        public bool IsLoadCalled { get; private set; }

        public ObservableCollection<PostItemViewModel> Posts { get; set; }

        private string _boardName;

        public string BoardName
        {
            get { return this._boardName; }
            set
            {
                this._boardName = value;
                this.OnPropertyChanged("BoardName");
            }
        }

        private string _threadNumber;

        public string ThreadNumber
        {
            get { return this._threadNumber; }
            set
            {
                this._threadNumber = value;
                this.OnPropertyChanged("ThreadNumber");
            }
        }

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
    }
}
