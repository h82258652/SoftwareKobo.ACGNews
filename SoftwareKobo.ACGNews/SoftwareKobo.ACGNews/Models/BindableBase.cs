using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;

namespace SoftwareKobo.ACGNews.Models
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        private static readonly CoreWindow CoreWindow = CoreWindow.GetForCurrentThread();

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual async void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (CoreWindow != null && CoreWindow.Dispatcher.HasThreadAccess == false)
            {
                await CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual bool Set<T>(ref T storage, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, newValue))
            {
                return false;
            }
            storage = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}