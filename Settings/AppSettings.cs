using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Models;
using MyDairy.Serialization;
using MyDairy.Services;
using Windows.Foundation;
using Windows.Graphics;

namespace MyDairy.Settings;

public partial class AppSettings : ObservableObject
{
    public const string SettingsFile = @"Settings.json";

    [JsonIgnore]
    public static readonly AppSettings Instance = LoadJson();

    #region Application Settings
    private ElementTheme _theme = ElementTheme.Default;
    [JsonConverter(typeof(JsonStringEnumConverter<ElementTheme>))]
    public ElementTheme Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    private BackdropType _backdropType = BackdropType.None;
    [JsonConverter(typeof(JsonStringEnumConverter<BackdropType>))]
    public BackdropType BackdropType
    {
        get => _backdropType;
        set => SetProperty(ref _backdropType, ThemeService.GetActualBackdropType(value));
    }

    private double _menuBarOpacity = 1;
    public double MenuBarOpacity
    {
        get => _menuBarOpacity;
        set => SetProperty(ref _menuBarOpacity, value);
    }

    private double _contentOpacity = 1;
    public double ContentOpacity
    {
        get => _contentOpacity;
        set => SetProperty(ref _contentOpacity, value);
    }

    public RectInt32? WindowPosition
    {
        get;
        set;
    } = null;
    #endregion

    #region Dairy Settings
    private DairyView _defaultDairyView = DairyView.CompactDoubleListView;
    [JsonConverter(typeof(JsonStringEnumConverter<DairyView>))]
    public DairyView DefaultDairyView
    {
        get => _defaultDairyView;
        set => SetProperty(ref _defaultDairyView, value);
    }

    private DairyPaneDisplayMode _dairyPaneDisplayMode = DairyPaneDisplayMode.Inline;
    [JsonConverter(typeof(JsonStringEnumConverter<DairyPaneDisplayMode>))]
    public DairyPaneDisplayMode DairyPaneDisplayMode
    {
        get => _dairyPaneDisplayMode;
        set => SetProperty(ref _dairyPaneDisplayMode, value);
    }

    private QuickNoteDisplayMode _quickNoteDisplayMode = QuickNoteDisplayMode.LeftPane;
    [JsonConverter(typeof(JsonStringEnumConverter<QuickNoteDisplayMode>))]
    public QuickNoteDisplayMode QuickNoteDisplayMode
    {
        get => _quickNoteDisplayMode;
        set => SetProperty(ref _quickNoteDisplayMode, value);
    }

    private bool _showTab = true;
    public bool ShowTab
    {
        get => _showTab;
        set => SetProperty(ref _showTab, value);
    }

    private bool _showDairyCount = true;
    public bool ShowDairyCount
    {
        get => _showDairyCount;
        set => SetProperty(ref _showDairyCount, value);
    }

    private bool _notShowDeleteAgain = false;
    public bool NotShowDeleteAgain
    {
        get => _notShowDeleteAgain;
        set => SetProperty(ref _notShowDeleteAgain, value);
    }

    private bool _notShowCloseAgain = false;
    public bool NotShowCloseAgain
    {
        get => _notShowCloseAgain;
        set => SetProperty(ref _notShowCloseAgain, value);
    }
    #endregion

    #region Note Settings
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<Note> Notes
    {
        get;
    } = [];
    #endregion

    public void Save()
    {
        var content = JsonSerializer.Serialize(this, SourceGenerationContext.Default.AppSettings);

        try
        {
            File.WriteAllText(SettingsFile, content);
        }
        catch (Exception)
        {

        }
    }
    public static AppSettings LoadJson()
    {
        try
        {
            var text = File.ReadAllText(SettingsFile);
            return JsonSerializer.Deserialize(text, SourceGenerationContext.Default.AppSettings);
        }
        catch (Exception)
        {
            return new();
        }
    }
}
