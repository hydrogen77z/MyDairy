using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MyDairy.Common;
using Windows.Foundation;

namespace MyDairy.Services;

public partial class RadioMenuService<T> where T : Enum
{
    private readonly Dictionary<T, RadioMenuFlyoutItem> _radios = [];

    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (!_value.Equals(value))
            {
                _value = value;

                SetIsChecked();
            }
        }
    }

    public event TypedEventHandler<RadioMenuService<T>, T> RadioChanged;

    public RadioMenuService(IList<RadioMenuFlyoutItem> radios, T defaultValue, [CallerMemberName]string groupName = null)
    {
        foreach (var radio in radios)
        {
            radio.GroupName = groupName;
            radio.Click += Radio_Click;
            _radios.Add(GetProperty(radio), radio);
        }

        _value = defaultValue;
        SetIsChecked();
    }

    private void SetIsChecked()
    {
        foreach (var item in _radios)
        {
            item.Value.IsChecked = item.Key.Equals(_value);
        }
    }

    private void Radio_Click(object sender, RoutedEventArgs e)
    {
        Value = GetProperty(sender);
        RadioChanged?.Invoke(this, _value);
    }

    private static T GetProperty(object element)
    {
        return (T)Enum.ToObject(typeof(T), Convert.ToInt32(((FrameworkElement)element).Tag));
    }
}
