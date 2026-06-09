using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDairy.Common;

public partial class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
    protected virtual bool SetProperty<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null, params string[] otherProperties)
    {
        if (EqualityComparer<T>.Default.Equals(value, newValue))
        {
            return false;
        }
        value = newValue;
        OnPropertyChanged(propertyName);

        foreach (var property in otherProperties)
        {
            OnPropertyChanged(property);
        }
        return true;
    }
}
