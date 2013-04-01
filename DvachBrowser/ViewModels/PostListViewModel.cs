using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Controls;
using DvachBrowser.Assets.HttpTasks;
using DvachBrowser.Models;
using DvachBrowser.Views;

using Microsoft.Phone.Tasks;

namespace DvachBrowser.ViewModels
{
    public class PostListViewModel : LoadingBaseViewModel
    {
        private readonly DvachUrlBuilder _urlBuilder;
        private readonly PageNavigationService _pageNavigationService;
        private readonly DvachUriParser _dvachUriParser;

        private readonly PostListPage _view;

        private HttpGetJsonTask<PostListModel> _currentTask;
        private HttpGetJsonTask<PostListModel> _updateTask;

        public PostListViewModel(PostListPage view)
        {
            this._view = view;

            this._dvachUriParser = Container.Resolve<DvachUriParser>();
            this.Posts = new ObservableCollection<PostItemViewModel>();
            this._urlBuilder = Container.Resolve<DvachUrlBuilder>();
            this._pageNavigationService = Container.Resolve<PageNavigationService>();
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
            string postsUrl = this._urlBuilder.BuildPostsUrl(boardName, threadNumber);
            this._currentTask = new HttpGetJsonTask<PostListModel>(postsUrl, this.OnPostsLoaded);
            this._currentTask.OnError = this.ShowError;
            this._currentTask.OnProgressChanged = this.UpdateProgress;

            this.ShowLoading();
            this._currentTask.Execute();
        }

        public void UpdatePosts()
        {
            if (this._updateTask != null)
            {
                this._updateTask.Cancel();
            }

            // load posts from the network
            string postsUrl = this._urlBuilder.BuildPostsUrl(this.BoardName, this.ThreadNumber);
            this._updateTask = new HttpGetJsonTask<PostListModel>(postsUrl, this.OnPostsLoadedAfterUpdate);
            this._updateTask.OnError = this.ShowErrorAfterUpdate;
            this._updateTask.OnProgressChanged = val => this.ProgressAfterUpdate = val;

            this.IsLoadingAfterUpdate = true;
            this._updateTask.Execute();
        }

        public void Refresh()
        {
            if (this.Posts.Count == 0)
            {
                this.Load(this.BoardName, this.ThreadNumber);
            }
            else
            {
                this.UpdatePosts();
            }
        }

        public override void ShowError(string message)
        {
            base.ShowError(message);

            this._currentTask = null;
        }

        private void ShowErrorAfterUpdate(string message)
        {
            MessageBox.Show(message);

            this.IsLoadingAfterUpdate = false;

            this._updateTask = null;
        }

        private void OnPostsLoaded(PostListModel responseObject)
        {
            this.DisplayPosts(responseObject);

            this.HideLoading();

            this._currentTask = null;
        }

        private void OnPostsLoadedAfterUpdate(PostListModel responseObject)
        {
            this.DisplayPosts(responseObject);

            this.IsLoadingAfterUpdate = false;

            this._updateTask = null;
        }

        private void DisplayPosts(PostListModel postList)
        {
            var lastPostNumber = this.Posts.Select(p => p.Number).DefaultIfEmpty(0).Max();
            var newPosts = postList.Posts.Select(postArray => postArray[0]).SkipWhile(post => post.Number <= lastPostNumber);
            int index = 1 + this.Posts.Select(p => p.Index).DefaultIfEmpty(0).Last();

            foreach (var post in newPosts)
            {
                var vm = new PostItemViewModel(this);
                vm.MapModel(post, index);

                this.Posts.Add(vm);

                index++;
            }
        }

        public void NavigateToLink(PostItemViewModel item, Hyperlink link)
        {
            Uri uri = link.GetValue(HyperlinkProperties.UriProperty) as Uri;

            if (this._dvachUriParser.IsDvachUri(uri))
            {
                uri = this._urlBuilder.FixRelativeUri(uri);

                var uriModel = this._dvachUriParser.ParseUri(uri);
                if (uriModel == null)
                {
                    return;
                }

                if (uriModel.BoardName == this.BoardName && uriModel.ThreadNumber == this.ThreadNumber)
                {
                    if (!string.IsNullOrEmpty(uri.Fragment))
                    {
                        string fragment = uri.Fragment.Substring(1);
                        var post = this.Posts.FirstOrDefault(p => p.Number.ToString() == fragment);

                        this._view.ScrollToPost(post);
                    }
                }
                else
                {
                    this._pageNavigationService.NavigateToPostListPage(uriModel.BoardName, uriModel.ThreadNumber);
                }
            }
            else
            {
                var webBrowserTask = new WebBrowserTask() { Uri = uri };
                webBrowserTask.Show();
            }
        }

        public void NavigateBoardsPage()
        {
            this._pageNavigationService.Navigate(Constants.BoardListPageUri);
        }

        public void NavigateAddPostPage()
        {
            string queryString = new QueryStringBuilder()
                .Add(Constants.QueryStringBoard, this.BoardName)
                .Add(Constants.QueryStringThread, this.ThreadNumber)
                .Build();

            this._pageNavigationService.Navigate(Constants.AddPostPageUri + queryString);
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

        private double _progressAfterUpdate;

        public double ProgressAfterUpdate
        {
            get { return this._progressAfterUpdate; }
            set
            {
                this._progressAfterUpdate = value;
                this.RaisePropertyChanged("ProgressAfterUpdate");
            }
        }

        private bool _isLoadingAfterUpdate;

        public bool IsLoadingAfterUpdate
        {
            get { return this._isLoadingAfterUpdate; }
            set
            {
                this._isLoadingAfterUpdate = value;
                this.RaisePropertyChanged("IsLoadingAfterUpdate");
            }
        }
    }
}
