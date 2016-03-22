using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class ValidatableObject : ObservableObject, INotifyDataErrorInfo
    {
        protected Dictionary<string, IList<string>> Errors;

        public bool HasErrors => Errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool IsValid
        {
            get
            {
                ValidateObject();
                return !HasErrors;
            }
        }

        protected ValidatableObject()
        {
            Errors = new Dictionary<string, IList<string>>();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !Errors.Keys.Contains(propertyName))
                return Enumerable.Empty<string>();

            return Errors[propertyName];
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && ErrorsChanged != null)
            {
                ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected bool ValidateAndSet<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            ValidateProperty(propertyName, newValue);
            return Set(propertyExpression, ref field, newValue);
        }

        protected void ValidateObject()
        {
            ClearErrors();

            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, true);

            if (results.Any())
            {
                var allProperties = results.SelectMany(x => x.MemberNames).Distinct().ToList();

                foreach (var propertyName in allProperties)
                {
                    Errors[propertyName] = results
                        .Where(x => x.MemberNames.Contains(propertyName))
                        .Select(x => x.ErrorMessage)
                        .Distinct()
                        .ToList();

                    OnErrorsChanged(propertyName);
                }
            }

            RaisePropertyChanged(nameof(HasErrors));
        }

        protected void ValidateProperty<T>(string propertyName, T newValue)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            var results = new List<ValidationResult>();
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            Validator.TryValidateProperty(newValue, context, results);

            if (results.Any())
            {
                Errors[propertyName] = results.Select(x => x.ErrorMessage).Distinct().ToList();
            }
            else
            {
                Errors.Remove(propertyName);
            }

            RaisePropertyChanged(nameof(HasErrors));
            OnErrorsChanged(propertyName);
        }

        protected void ClearErrors()
        {
            foreach (var propertyName in Errors.Keys.ToList())
            {
                Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
