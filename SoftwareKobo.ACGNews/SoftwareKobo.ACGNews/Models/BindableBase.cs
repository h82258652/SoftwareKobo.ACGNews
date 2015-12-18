using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoftwareKobo.ACGNews.Models
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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