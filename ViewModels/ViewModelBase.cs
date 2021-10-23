using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Zivver.ViewModels
{
    /// <summary>
    /// This method exists so we don't have to add same code to every ViewModel class for OnPropertyChanged
    /// Also it's good because than we don't need to worry if event has been raised on every change of property 
    /// </summary>

    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        // this is needed for preventing memory leak 
        public virtual void Dispose() { }
    }
}
