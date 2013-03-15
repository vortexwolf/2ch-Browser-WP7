using System;
using System.Collections.ObjectModel;
using System.Linq;

using DvachBrowser.Assets;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class PostListViewModel : ListBaseViewModel
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
            if (this._currentTask != null)
            {
                this._currentTask.Cancel();
            }

            this.BoardName = boardName;
            this.ThreadNumber = threadNumber;
            this.Title = "/" + boardName + "/" + threadNumber;

            // load posts from the network
            string postsUrl = string.Format("http://2ch.hk/{0}/res/{1}.json?nocache={2}", boardName, threadNumber, DateTime.UtcNow);
            this._currentTask = new HttpGetJsonTask<PostListModel>(postsUrl, this.OnPostsLoaded);
            this._currentTask.OnError = this.ShowError;
            this._currentTask.OnProgressChanged = this.UpdateProgress;

            this.ShowLoading();
            this._currentTask.Execute();
        }

        public void Refresh()
        {
            this.Load(this.BoardName, this.ThreadNumber);
        }

        private void OnPostsLoaded(PostListModel responseObject)
        {
            this.DisplayPosts(responseObject);

            this.HideLoading();

            this._currentTask = null;
        }

        protected override void ShowError(string message)
        {
            base.ShowError(message);

            this._currentTask = null;
        }

        private void DisplayPosts(PostListModel postList)
        {
            var lastPostNumber = this.Posts.Select(p => p.Number).DefaultIfEmpty(0).Max();
            var newPosts = postList.Posts.Select(postArray => postArray[0]).SkipWhile(post => post.Number <= lastPostNumber);
            int index = 1 + this.Posts.Select(p => p.Index).DefaultIfEmpty(0).Last();

            foreach (var post in newPosts)
            {
                var vm = new PostItemViewModel(this.BoardName, this._bitmapManager);
                vm.MapModel(post, index);

                this.Posts.Add(vm);

                index++;
            }
        }

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
    }
}
