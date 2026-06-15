using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using MyDairy.Common;

namespace MyDairy.Services;

public partial class WindowExtendedConfiguration : ObservableObject
{
    private BackdropType _backdropType = BackdropType.None;
    public BackdropType BackdropType
    {
        get => _backdropType;
        set => SetProperty(ref _backdropType, value);
    }

    private ElementTheme _requestedTheme = ElementTheme.Default;
    public ElementTheme RequestedTheme
    {
        get => _requestedTheme;
        set => SetProperty(ref _requestedTheme, value);
    }

    private bool _useInterceptTitleBarCaptionAreaChanged = false;
    /// <summary>
    /// Intercept changes to the title area of the title bar, and reset the new area when the title area is changed.
    /// defaultValue: <b>false</b>
    /// </summary>
    public bool UseInterceptTitleBarCaptionAreaChanged
    {
        get => _useInterceptTitleBarCaptionAreaChanged;
        set => SetProperty(ref _useInterceptTitleBarCaptionAreaChanged, value);
    }
}
