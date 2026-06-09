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
using Windows.UI.ViewManagement;
using WinRT;
using WinRT.Interop;

namespace MyDairy.Services;

public partial class WindowService
{
    private BackdropType _backdropType = BackdropType.None;
    private ISystemBackdropControllerWithTargets _controller = null;
    private readonly ICompositionSupportsSystemBackdrop _windowBackdrop = null;

    private readonly Window _window;
    private readonly SystemBackdropConfiguration _config = new();
    private readonly DesktopAcrylicController _acrylic = new();
    private readonly MicaController _mica = new();
    private readonly OverlappedPresenter _presenter = null;
    private InputNonClientPointerSource _inputNonClientPointerSource;

    private bool _enabled = false;
    private bool IsEnabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            SetInputActive();
        }
    }

    private bool _isActived = false;
    private bool IsActive
    {
        get => _isActived;
        set
        {
            _isActived = value;
            SetInputActive();
        }
    }

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
        _config.IsInputActive = _enabled && _isActived;
    }
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        IsActive = args.WindowActivationState != WindowActivationState.Deactivated;
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
        IsEnabled = true;
    }
    private void SwitchMica(MicaKind kind)
    {
        if (_controller is not MicaController)
        {
            _controller?.RemoveSystemBackdropTarget(_windowBackdrop);
            SetController(_mica);
        }
        _mica.Kind = kind;
        IsEnabled = true;
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
        IsEnabled = false;
    }

    private void SetController(ISystemBackdropControllerWithTargets controller)
    {
        _controller = controller;
        controller.AddSystemBackdropTarget(_windowBackdrop);
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        _window.Activated -= OnActivated;
        _mica?.Dispose();
        _acrylic?.Dispose();
    }

    public BackdropType BackdropType
    {
        get => _backdropType;
        set
        {
            var actualValue = ThemeService.GetActualBackdropType(value);
            if (_backdropType != actualValue)
            {
                _backdropType = actualValue;

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
        }
    }

    public bool IsAlwaysOnTop
    {
        get => _presenter.IsAlwaysOnTop;
        set => _presenter.IsAlwaysOnTop = value;
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

    private bool _useInterceptTitleBarCaptionAreaChanged = false;
    /// <summary>
    /// Intercept changes to the title area of the title bar, and reset the new area when the title area is changed.
    /// defaultValue: <b>false</b>
    /// </summary>
    public bool UseInterceptTitleBarCaptionAreaChanged
    {
        get => _useInterceptTitleBarCaptionAreaChanged;
        set
        {
            if (_useInterceptTitleBarCaptionAreaChanged != value)
            {
                _useInterceptTitleBarCaptionAreaChanged = value;

                _inputNonClientPointerSource ??= InputNonClientPointerSource.GetForWindowId(_window.AppWindow.Id);

                if (value)
                {
                    _inputNonClientPointerSource.RegionsChanged += InputNonClientPointerSource_RegionsChanged;
                }
                else
                {
                    _inputNonClientPointerSource.RegionsChanged -= InputNonClientPointerSource_RegionsChanged;
                }
            }
        }
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

    public WindowService(Window window)
    {
        _window = window;
        _windowBackdrop = window.As<ICompositionSupportsSystemBackdrop>();

        var element = (FrameworkElement)_window.Content;

        _window.Closed += OnWindowClosed;
        _window.Activated += OnActivated;
        element.ActualThemeChanged += OnActualThemeChanged;
        _acrylic.SetSystemBackdropConfiguration(_config);
        _mica.SetSystemBackdropConfiguration(_config);

        UpdateAcrylicColor(element.ActualTheme);
        OnActualThemeChanged(element, null);

        _presenter = _window.AppWindow.Presenter.As<OverlappedPresenter>();

        SwitchNone();
    }
}
