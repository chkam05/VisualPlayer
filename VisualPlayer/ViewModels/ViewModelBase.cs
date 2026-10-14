using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VisualPlayer.Attributes;

namespace VisualPlayer.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    private BindingFlags _propertyFlags
        = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    
    public event PropertyChangedEventHandler? PropertyChanged;


    protected virtual bool CheckNotifyPropertyUpdateAttribute(string propertyName)
    {
        var property = GetType().GetProperty(propertyName, _propertyFlags);

        if (property is null)
            return false;

        return property.GetCustomAttribute<NotifyPropertyUpdateAttribute>(
            inherit: true) is not null;
    }

    protected virtual void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void UpdateProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
            return;

        field = newValue;

        if (string.IsNullOrWhiteSpace(propertyName))
            return;

        if (!CheckNotifyPropertyUpdateAttribute(propertyName))
            return;

        NotifyPropertyChanged(propertyName);
    }
}