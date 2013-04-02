using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private readonly PopupDisplayer _popupDisplayer;

        private readonly Panel _popupPlaceholder;

        private readonly Dictionary<long, PostItemViewModel> _itemsByPostNumbers; 

        private HttpGetJsonTask<PostListModel> _currentTask;
        private HttpGetJsonTask<PostListModel> _updateTask;

        public PostListViewModel(Panel popupPlaceholder)
        {
            this._popupPlaceholder = popupPlaceholder;

            this.Posts = new ObservableCollection<PostItemViewModel>();
            this._itemsByPostNumbers = new Dictionary<long, PostItemViewModel>();

            this._dvachUriParser = Container.Resolve<DvachUriParser>();
            this._urlBuilder = Container.Resolve<DvachUrlBuilder>();
            this._pageNavigationService = Container.Resolve<PageNavigationService>();
            this._popupDisplayer = Container.Resolve<PopupDisplayer>();
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
            this.AddPosts(responseObject);

            this.HideLoading();

            this._currentTask = null;
        }

        private void OnPostsLoadedAfterUpdate(PostListModel responseObject)
        {
            this.AddPosts(responseObject);

            this.IsLoadingAfterUpdate = false;

            this._updateTask = null;
        }

        private void AddPosts(PostListModel postList)
        {
            int index = this.Posts.Count + 1;
            var lastPostNumber = this.Posts.Select(p => p.Number).DefaultIfEmpty(0).Max();
            var newPosts = postList.Posts.Select(postArray => postArray[0]).SkipWhile(post => post.Number <= lastPostNumber);

            foreach (var post in newPosts)
            {
                this.AddModel(post, index);

                index++;
            }
        }

        private void AddModel(PostItemModel model, int index)
        {
            var vm = new PostItemViewModel(this);

            vm.MapModel(model, index);
            this.ArrangeReferences(vm);

            this.Posts.Add(vm);
            this._itemsByPostNumbers[vm.Number] = vm;
        }
        
        private void ArrangeReferences(PostItemViewModel model)
        {
            foreach (var refNumber in model.RefersTo)
            {
                if (this._itemsByPostNumbers.ContainsKey(refNumber))
                {
                    this._itemsByPostNumbers[refNumber].AddReferenceFrom(model.Number);
                }
            }
        }

        public void ShowReplies(PostItemViewModel item)
        {
            var replies = item.ReferencesFrom.Select(number => this._itemsByPostNumbers[number]).ToList();

            this._popupDisplayer.ShowPosts(replies, this._popupPlaceholder);
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
                    this.NavigateToBrowser(uri);
                    return;
                }

                // if uri navigates to the current thread
                if (uriModel.BoardName == this.BoardName && uriModel.ThreadNumber == this.ThreadNumber)
                {
                    // don't do anything if the uri has no fragment
                    if (!string.IsNullOrEmpty(uri.Fragment))
                    {
                        string fragment = uri.Fragment.Substring(1);
                        var post = this.Posts.FirstOrDefault(p => p.Number.ToString() == fragment);

                        this._popupDisplayer.ShowPost(post, this._popupPlaceholder);
                    }
                }
                else
                {
                    // if uri navigates to another thread, open the necessary page
                    this._pageNavigationService.NavigateToPostListPage(uriModel.BoardName, uriModel.ThreadNumber);
                }
            }
            else
            {
                this.NavigateToBrowser(uri);
            }
        }

        private void NavigateToBrowser(Uri uri)
        {
            var webBrowserTask = new WebBrowserTask() { Uri = uri };
            webBrowserTask.Show();
        }

        public void NavigateBoardsPage()
        {
            this._pageNavigationService.Navigate(Constants.BoardListPageUri);
        }

        public void NavigateAddPostPage()
        {
            this._pageNavigationService.NavigateToAddPostPage(this.BoardName, this.ThreadNumber);
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
