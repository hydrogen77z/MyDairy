using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ABI.Windows.AI.MachineLearning;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MyDairy.Common;
using MyDairy.Helpers;
using Windows.UI;
using Windows.UI.ViewManagement;
using WinRT;
using WinRT.Interop;

namespace MyDairy.Services;

public partial class WindowService
{
    private BackdropType _backdropType = BackdropType.None;
    private bool _useInterceptTitleBarCaptionAreaChanged = false;

    private ISystemBackdropControllerWithTargets _controller = null;
    private readonly ICompositionSupportsSystemBackdrop _windowBackdrop = null;

    private readonly Window _window;
    private readonly WindowExtendedConfiguration _extendedConfiguration;
    private readonly SystemBackdropConfiguration _config = new();
    private readonly DesktopAcrylicController _acrylic = new();
    private readonly MicaController _mica = new();
    private readonly OverlappedPresenter _presenter = null;
    private InputNonClientPointerSource _inputNonClientPointerSource;

    private bool _isEnabled = false;
    private bool _isActived = false;

    private void UpdateTitleBarColor(ElementTheme theme)
    {
        var buttonColor = (byte)(theme == ElementTheme.Dark ? 0xff : 0);
        _window.AppWindow.TitleBar.ButtonForegroundColor = new()
        {
            A = 0xff,
            R = buttonColor,
            G = buttonColor,
            B = buttonColor,
        };
    }
    private void UpdateAcrylicColor(ElementTheme actualTheme)
    {
        var acrylicColor = (byte)(actualTheme == ElementTheme.Dark ? 0x20 : 0xf3);
        _acrylic.FallbackColor = _acrylic.TintColor = new()
        {
            A = 0xff,
            R = acrylicColor,
            G = acrylicColor,
            B = acrylicColor,
        };
    }
    private void SetInputActive()
    {
        _config.IsInputActive = _isEnabled && _isActived;
    }
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        _isActived = args.WindowActivationState != WindowActivationState.Deactivated;
        SetInputActive();
    }
    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        var theme = sender.ActualTheme;

        _config.Theme = (SystemBackdropTheme)theme;
        UpdateTitleBarColor(theme);
        UpdateAcrylicColor(theme);
    }

    private void SwitchAcrylic()
    {
        if (_controller is not DesktopAcrylicController)
        {
            _controller?.RemoveSystemBackdropTarget(_windowBackdrop);
            SetController(_acrylic);
        }
        _isEnabled = true;
        SetInputActive();
    }
    private void SwitchMica(MicaKind kind)
    {
        if (_controller is not MicaController)
        {
            _controller?.RemoveSystemBackdropTarget(_windowBackdrop);
            SetController(_mica);
        }
        _mica.Kind = kind;
        _isEnabled = true;
        SetInputActive();
    }
    private void SwitchNone()
    {
        if (_controller is null)
        {
            if (MicaController.IsSupported())
            {
                SwitchMica(MicaKind.Base);
            }
            else
            {
                SwitchAcrylic();
            }
        }
        _isEnabled = false;
        SetInputActive();
    }

    private void SetController(ISystemBackdropControllerWithTargets controller)
    {
        _controller = controller;
        controller.AddSystemBackdropTarget(_windowBackdrop);
    }
    private void SetBackdropType(BackdropType backdropType)
    {
        _backdropType = ThemeService.GetActualBackdropType(backdropType);

        switch (_backdropType)
        {
            case BackdropType.Acrylic:
                SwitchAcrylic();
                break;
            case BackdropType.Mica:
                SwitchMica(MicaKind.Base);
                break;
            case BackdropType.MicaAlt:
                SwitchMica(MicaKind.BaseAlt);
                break;
            default:
                SwitchNone();
                break;
        }
    }
    private void SetRequestedTheme(ElementTheme requestedTheme)
    {
        _window.Content.As<FrameworkElement>().RequestedTheme = requestedTheme;
    }
    private void SetUseInterceptTitleBarCaptionAreaChanged(bool useInterceptTitleBarCaptionAreaChanged)
    {
        if (_useInterceptTitleBarCaptionAreaChanged != useInterceptTitleBarCaptionAreaChanged)
        {
            _useInterceptTitleBarCaptionAreaChanged = useInterceptTitleBarCaptionAreaChanged;

            _inputNonClientPointerSource ??= InputNonClientPointerSource.GetForWindowId(_window.AppWindow.Id);

            if (useInterceptTitleBarCaptionAreaChanged)
            {
                _inputNonClientPointerSource.RegionsChanged += InputNonClientPointerSource_RegionsChanged;
            }
            else
            {
                _inputNonClientPointerSource.RegionsChanged -= InputNonClientPointerSource_RegionsChanged;
            }
        }
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        _extendedConfiguration.PropertyChanged -= OnExtendedConfigurationPropertyChanged;
        _window.Content.As<FrameworkElement>().ActualThemeChanged -= OnActualThemeChanged;
        _window.Activated -= OnActivated;
        _window.Closed -= OnWindowClosed;
        _mica?.Dispose();
        _acrylic?.Dispose();
    }

    private bool _regionReset = false;
    private void InputNonClientPointerSource_RegionsChanged(InputNonClientPointerSource sender, NonClientRegionsChangedEventArgs args)
    {
        var regionArray = args.ChangedRegions;
        var region = regionArray[0];

        if (region == NonClientRegionKind.Caption)
        {
            var rects = sender.GetRegionRects(NonClientRegionKind.Caption);
            if (rects.Length == 0)
            {
                return;
            }
            var rect = rects[0];
        
            if (rect.Height == 0 && _regionReset == false)
            {
                _regionReset = true;
                rect.Width = _window.AppWindow.Size.Width;
                rect.Height = 32;
                sender.SetRegionRects(NonClientRegionKind.Caption, [rect]);
            }
            else
            {
                _regionReset = false;
            }
        }

        // record
        //if (region == NonClientRegionKind.Caption || region == NonClientRegionKind.Passthrough)
        //{
        //    var changed = sender.GetRegionRects(region);
        //    foreach (var item in changed)
        //    {
        //        Debug.WriteLine($"{region}: X:{item.X}, Y:{item.Y}, Width:{item.Width}, Height:{item.Height}");
        //    }
        //}
    }

    public int? MinHeight
    {
        get => _presenter.PreferredMinimumHeight;
        set => _presenter.PreferredMinimumHeight = value;
    }
    public int? MaxHeight
    {
        get => _presenter.PreferredMaximumHeight;
        set => _presenter.PreferredMaximumHeight = value;
    }
    public int? MinWidth
    {
        get => _presenter.PreferredMinimumWidth;
        set => _presenter.PreferredMinimumWidth = value;
    }
    public int? MaxWidth
    {
        get => _presenter.PreferredMaximumWidth;
        set => _presenter.PreferredMaximumWidth = value;
    }

    public WindowService(Window window, WindowExtendedConfiguration configuration)
    {
        _window = window;
        //_windowId = window.AppWindow.Id;
        _windowBackdrop = window.As<ICompositionSupportsSystemBackdrop>();

        var element = window.Content.As<FrameworkElement>();

        _extendedConfiguration = configuration;

        _acrylic.SetSystemBackdropConfiguration(_config);
        _mica.SetSystemBackdropConfiguration(_config);

        _presenter = window.AppWindow.Presenter.As<OverlappedPresenter>();

        SetBackdropType(_extendedConfiguration.BackdropType);
        SetRequestedTheme(_extendedConfiguration.RequestedTheme);
        SetUseInterceptTitleBarCaptionAreaChanged(_extendedConfiguration.UseInterceptTitleBarCaptionAreaChanged);
        OnActualThemeChanged(element, null);

        // Add Event
        _extendedConfiguration.PropertyChanged += OnExtendedConfigurationPropertyChanged;
        window.Closed += OnWindowClosed;
        window.Activated += OnActivated;
        element.ActualThemeChanged += OnActualThemeChanged;
    }

    private void OnExtendedConfigurationPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(WindowExtendedConfiguration.BackdropType):
                SetBackdropType(_extendedConfiguration.BackdropType);
                break;
            case nameof(WindowExtendedConfiguration.RequestedTheme):
                SetRequestedTheme(_extendedConfiguration.RequestedTheme);
                break;
            case nameof(WindowExtendedConfiguration.UseInterceptTitleBarCaptionAreaChanged):
                SetUseInterceptTitleBarCaptionAreaChanged(_extendedConfiguration.UseInterceptTitleBarCaptionAreaChanged);
                break;
        }
    }
}
