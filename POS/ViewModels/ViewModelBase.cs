using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace POS.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected T SetProperty<T>(T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return storage;

            storage = value;
            OnPropertyChanged(propertyName);
            return value;
        }

        public T SetProperty<T>(T storage, T value, Expression<Func<object>> mainProperty, params Expression<Func<object>>[] additionalProperties)
        {
            var propertyName = GetPropertyNameFromLamda(mainProperty);
            if (SetProperty(ref storage, value, propertyName))
            {
                NotifyPropertyChanged(additionalProperties);
                return value;
            }
            else
                return storage;
        }

        private string GetPropertyNameFromLamda(LambdaExpression lambda)
        {
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            return propertyInfo.Name;
        }

        public void NotifyPropertyChanged(params Expression<Func<object>>[] properties)
        {
            foreach (var prop in properties)
                OnPropertyChanged(GetPropertyNameFromLamda(prop));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
