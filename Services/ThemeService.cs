using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using MyDairy.Common;

namespace MyDairy.Services;

public partial class ThemeService : ObservableObject
{
    public static readonly ThemeService Instance = new();

    private readonly UISettings _uiSettings = new();
    private readonly DispatcherQueue _queue = DispatcherQueue.GetForCurrentThread();

    public static BackdropType GetActualBackdropType(BackdropType type)
    {
        if (!Instance._isTransparencyEnabled)
        {
            return BackdropType.None;
        }

        if ((type == BackdropType.Mica || type == BackdropType.MicaAlt) && MicaController.IsSupported())
        {
            return type;
        }
        else if (type == BackdropType.Acrylic && DesktopAcrylicController.IsSupported())
        {
            return type;
        }
        return BackdropType.None;
    }

    private bool _isTransparencyEnabled = false;
    public bool IsTransparencyEnabled
    {
        get => _isTransparencyEnabled;
        private set => SetProperty(ref _isTransparencyEnabled, value);
    }

    public static readonly bool IsSupportAcrylic = DesktopAcrylicController.IsSupported();
    public static readonly bool IsSupportMica = MicaController.IsSupported();

    public ThemeService()
    {
        _isTransparencyEnabled = _uiSettings.AdvancedEffectsEnabled;
        _uiSettings.AdvancedEffectsEnabledChanged += (sender, args) =>
        {
            _queue.TryEnqueue(() => IsTransparencyEnabled = sender.AdvancedEffectsEnabled);
        };
    }
}
