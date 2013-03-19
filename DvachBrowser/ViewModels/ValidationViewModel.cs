using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

using DvachBrowser.Assets.Validation;

namespace DvachBrowser.ViewModels
{
    public class ValidationViewModel : ViewModel, INotifyDataErrorInfo
    {
        public ValidationViewModel()
        {
            this.PropertyChanged += (s, e) =>
            {
                // if the changed property is one of the properties which require validation
                if (this._validator.PropertyNames.Contains(e.PropertyName))
                {
                    this._validator.ValidateProperty(e.PropertyName);
                    this.OnErrorsChanged(e.PropertyName);
                }
            };
        }

        protected ModelValidator _validator = new ModelValidator();

        public bool ValidateAll()
        {
            var result = this._validator.ValidateAll();

            this._validator.PropertyNames.ToList().ForEach(this.OnErrorsChanged);

            return result;
        }
        
        public IEnumerable GetErrors(string propertyName)
        {
            return this._validator.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return this._validator.ErrorMessages.Count > 0; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        protected virtual void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));

            this.RaisePropertyChanged("HasErrors");
        }
    }
}
