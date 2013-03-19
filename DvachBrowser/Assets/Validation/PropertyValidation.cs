using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using DvachBrowser.Assets.Extensions;

namespace DvachBrowser.Assets.Validation
{
    public class PropertyValidation<TP> : IPropertyValidation
    {
        private const string DefaultValidationError = "Validation error";
        private readonly List<Func<bool>> _validationCriterias = new List<Func<bool>>();

        private readonly Func<TP> _getPropertyValue;

        private Func<bool> _validationPrecondition;

        public PropertyValidation(Expression<Func<TP>> expression)
        {
            this._getPropertyValue = expression.Compile();
            this.PropertyName = PropertyNameExtensions.GetPropertyName(expression);
            this.ErrorMessage = DefaultValidationError;
        }

        public string PropertyName { get; private set; }

        public string ErrorMessage { get; private set; }

        public TP PropertyValue 
        { 
            get { return this._getPropertyValue(); } 
        }

        /// <summary>
        /// Sets the condition whether to validate or not.
        /// </summary>
        public PropertyValidation<TP> When(Func<bool> validationPrecondition)
        {
            this._validationPrecondition = validationPrecondition;
            return this;
        }

        /// <summary>
        /// Sets the error message when the property is invalid.
        /// </summary>
        public PropertyValidation<TP> Show(string errorMessage)
        {
            if (this.ErrorMessage != null && this.ErrorMessage != DefaultValidationError)
            {
                throw new InvalidOperationException("You can set the error message only once.");
            }

            this.ErrorMessage = errorMessage;
            return this;
        }

        /// <summary>
        /// Validates the property using applied criteria.
        /// </summary>
        public bool IsInvalid()
        {
            if (this._validationCriterias.Count == 0)
            {
                throw new InvalidOperationException("No criteria has been provided for this validation.");
            }

            if (this._validationPrecondition != null && !this._validationPrecondition())
            {
                return false;
            }

            return this._validationCriterias.Any(f => !f());
        }

        /// <summary>
        /// Sets the rule that the property shouldn't be null.
        /// </summary>
        public PropertyValidation<TP> NotNull()
        {
            this._validationCriterias.Add(this.CheckIsNotNullValue);
            return this;
        }

        /// <summary>
        /// Sets the rule that the string property shouldn't be null or empty.
        /// </summary>
        public PropertyValidation<TP> NotEmpty()
        {
            this._validationCriterias.Add(this.CheckIsNotEmptyValue);
            return this;
        }

        /// <summary>
        /// Sets the rule that the property should satisfy custom criterion.
        /// </summary>
        public PropertyValidation<TP> Must(Func<bool> validationCriteria)
        {
            this._validationCriterias.Add(validationCriteria);
            return this;
        }

        private bool CheckIsNotNullValue()
        {
            return this.PropertyValue != null;
        }

        private bool CheckIsNotEmptyValue()
        {
            object val = this.PropertyValue;

            if (val is string)
            {
                return !string.IsNullOrWhiteSpace((string)val);
            }

            return this.CheckIsNotNullValue();
        }
    }
}
