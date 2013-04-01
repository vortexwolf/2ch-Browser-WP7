using System;
using System.Collections.ObjectModel;
using DvachBrowser.Assets;
using DvachBrowser.Assets.HttpTasks;
using DvachBrowser.Models;

namespace DvachBrowser.ViewModels
{
    public class ThreadListViewModel : LoadingBaseViewModel
    {
        private readonly BitmapManager _bitmapManager;
        private readonly DvachUrlBuilder _urlBuilder;

        private HttpGetJsonTask<ThreadListModel> _currentTask;

        public ThreadListViewModel()
        {
            this.Threads = new ObservableCollection<ThreadItemViewModel>();
            this._bitmapManager = Container.Resolve<BitmapManager>();
            this._urlBuilder = Container.Resolve<DvachUrlBuilder>();
        }

        public void Load(string boardName, int page)
        {
            if (this._currentTask != null)
            {
                this._currentTask.Cancel();
            }

            this.BoardName = boardName;
            this.Page = page;
            this.Title = string.Format("/{0}/{1}", boardName, page != 0 ? page.ToString() : string.Empty);

            // load threads from the network
            string threadsUrl = this._urlBuilder.BuildThreadsUrl(boardName, page);
            this._currentTask = new HttpGetJsonTask<ThreadListModel>(threadsUrl, this.OnThreadsLoaded);
            this._currentTask.OnError = this.ShowError;
            this._currentTask.OnProgressChanged = this.UpdateProgress;

            this.ShowLoading();
            this._currentTask.Execute();
        }

        public void Refresh()
        {
            this.Load(this.BoardName, this.Page);
        }

        public override void ShowError(string message)
        {
            base.ShowError(message);

            this._currentTask = null;
        }

        private void OnThreadsLoaded(ThreadListModel responseObject)
        {
            this.DisplayThreads(responseObject);

            this.HideLoading();

            this._currentTask = null;
        }
        
        private void DisplayThreads(ThreadListModel threadList)
        {
            this.Threads.Clear();

            foreach (var thread in threadList.Threads)
            {
                var vm = new ThreadItemViewModel(this.BoardName, thread);

                this.Threads.Add(vm);
            }
        }

        public string BoardName { get; set; }

        public int Page { get; set; }

        public ObservableCollection<ThreadItemViewModel> Threads { get; set; }
    }
}
