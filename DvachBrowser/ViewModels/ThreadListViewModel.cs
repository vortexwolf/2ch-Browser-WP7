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
            string threadsUrl = string.Format("http://2ch.hk/{0}/wakaba.json?nocache={1}", boardName, DateTime.UtcNow.Ticks);
            this._currentTask = new HttpGetJsonTask<ThreadListModel>(threadsUrl, this.OnThreadsLoaded);
            this._currentTask.OnError = this.ShowError;
            this._currentTask.OnProgressChanged = this.UpdateProgress;

            this.ShowLoading();
            this._currentTask.Execute();
        }

        public void Refresh()
        {
            this.Load(this.BoardName);
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
                var vm = new ThreadItemViewModel(this.BoardName, thread, this._bitmapManager);

                this.Threads.Add(vm);
            }
        }

        public string BoardName { get; set; }

        public ObservableCollection<ThreadItemViewModel> Threads { get; set; }
    }
}
