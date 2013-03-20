using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DvachBrowser.Assets.HttpTasks
{
    public abstract class HttpBaseTask
    {
        protected bool _isCancelled;
        protected bool _isExecuted;

        public virtual void Execute()
        {
            if (this._isExecuted)
            {
                throw new NotSupportedException();
            }

            this._isExecuted = true;
        }

        public void Cancel()
        {
            this._isCancelled = true;
        }

        protected void InvokeOnErrorHandler(string message)
        {
            if (this.OnError != null)
            {
                this.InvokeInUiThread(() => this.OnError(message));
            }
        }

        protected void InvokeInUiThread(Action action)
        {
            if (!this._isCancelled)
            {
                Deployment.Current.Dispatcher.BeginInvoke(action);
            }
        }

        public Action<string> OnError { get; set; }
    }
}
