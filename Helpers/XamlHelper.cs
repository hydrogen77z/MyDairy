using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using MyDairy.Common;
using MyDairy.Models;

namespace MyDairy.Helpers;

public static class XamlHelper
{
    public static bool BoolNot(bool value) => !value;

    public static Visibility ToVisible(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
    public static Visibility ToCollapsed(bool value) => ToVisible(!value);

    //public static Visibility NotNullToVisible(object value) => value == null ? Visibility.Collapsed : Visibility.Visible;
    //public static bool NotNullToEnable(object value) => value != null;

    public static bool And(bool value1, bool value2) => value1 && value2;
    public static bool Or(bool value1, bool value2) => value1 || value2;

    private static readonly ResourceLoader _resourceLoader = new();
    private static readonly IDictionary<string, ResourceLoader> _resourceFiles = new Dictionary<string, ResourceLoader>();

    public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);
    public static string GetLocalized(this string resourceKey, string resourceFile)
    {
        if (resourceFile == null)
        {
            return resourceKey.GetLocalized();
        }

        if (_resourceFiles.TryGetValue(resourceFile, out var loader))
        {
            return loader.GetString(resourceKey);
        }

        var newLoader = new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), resourceFile);
        _resourceFiles.Add(resourceFile, newLoader);
        return newLoader.GetString(resourceKey);
    }

    public static string DateTimeToString(DateTime dateTime) => dateTime.ToString(GlobalConstants.DateTimeFormat, CultureInfo.InvariantCulture);

    public static string DateOnlyToString(DateOnly dateOnly) => dateOnly.ToString(GlobalConstants.DateOnlyFormat);

    public static string DateAndTimeToString(DateOnly dateOnly, TimeOnly? timeOnly)
    {
        if (timeOnly.HasValue)
        {
            return DateTimeToString(new(dateOnly, timeOnly.Value));
        }
        else
        {
            return string.Format(GlobalConstants.DateAndTimeNullFormat, dateOnly);
        }
    }

    public static string GetDairyDate(object dairyDay)
    {
        if (dairyDay is null)
        {
            return string.Empty;
        }

        return DateOnlyToString(((DairyDay)dairyDay).Date);
    }

    public static string IsSavedToGlyph(bool value) => value ? string.Empty : "*";

    public static bool NotNullStringToEnable(string value) => !string.IsNullOrEmpty(value);

    public static bool BoolNullToTrue(bool? value) => value == true;

    public static bool IsValidOrCollapsed(bool value, Visibility visible)
    {
        return value || visible == Visibility.Collapsed;
    }
}
