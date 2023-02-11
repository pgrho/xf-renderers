using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty(ref string field, string value, Action onChanged = null, [CallerMemberName] string propertyName = null)
    {
        if (value != field)
        {
            field = value;
            if (propertyName != null)
            {
                RaisePropertyChanged(propertyName);
            }
            onChanged?.Invoke();
            return true;
        }
        return false;
    }

    protected bool SetProperty<T>(ref T field, T value, Action onChanged = null, [CallerMemberName] string propertyName = null)
    {
        if (!((field as IEquatable<T>)?.Equals(value) ?? Equals(field, value)))
        {
            field = value;
            if (propertyName != null)
            {
                RaisePropertyChanged(propertyName);
            }
            onChanged?.Invoke();
            return true;
        }
        return false;
    }
}
