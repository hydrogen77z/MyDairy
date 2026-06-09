using System;
using Microsoft.UI.Xaml.Data;

namespace MyDairy.Common.Converters;

public partial class EnumIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return System.Convert.ToInt32(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
