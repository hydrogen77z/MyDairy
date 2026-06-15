using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using MyDairy.Common;
using MyDairy.Helpers;
using Windows.Foundation;
using WinRT;

namespace MyDairy.Services;

public partial class ContentDialogService
{
    private readonly ContentDialog _contentDialog;
    private readonly ContentDialogInfo _contentDialogInfo;
    private readonly long _token;

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        _contentDialog.Title = null;
        _contentDialog.Content = null;

        _contentDialog.Closed -= OnContentDialogClosed;
        _contentDialog.PrimaryButtonClick -= OnContentDialogPrimaryButtonClick;
        _contentDialogInfo.UnregisterPropertyChangedCallback(ContentDialogInfo.IsPrimaryButtonEnabledProperty, _token);
    }

    private void OnIsPrimaryButtonEnabledChanged()
    {
        _contentDialog.IsPrimaryButtonEnabled = _contentDialogInfo.IsPrimaryButtonEnabled;
    }

    public ContentDialogService(ContentDialogInfo dialogInfo, XamlRoot xamlRoot)
    {
        _contentDialogInfo = dialogInfo;
        _contentDialog = new()
        {
            Title = _contentDialogInfo.Title,
            Content = _contentDialogInfo.Content,
            IsPrimaryButtonEnabled = _contentDialogInfo.IsPrimaryButtonEnabled,
            XamlRoot = xamlRoot,
        };
        SetDialogKind(_contentDialog, _contentDialogInfo.Kind);

        _token = _contentDialogInfo.RegisterPropertyChangedCallback(ContentDialogInfo.IsPrimaryButtonEnabledProperty, (d, e) => OnIsPrimaryButtonEnabledChanged());

        _contentDialog.Closed += OnContentDialogClosed;
        _contentDialog.PrimaryButtonClick += OnContentDialogPrimaryButtonClick;
    }

    private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        _contentDialogInfo.RaiseValidatingEvent(this, args);
    }

    public void SetIsEnabled(bool isEnabled)
    {
        _contentDialog.IsPrimaryButtonEnabled = isEnabled;
        if (isEnabled)
        {
            _contentDialog.Closing -= PreventDialogClosing;
        }
        else
        {
            _contentDialog.Closing += PreventDialogClosing;
        }
    }

    private void PreventDialogClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        args.Cancel = true;
    }

    public void Close() => _contentDialog.Hide();
    public IAsyncOperation<ContentDialogResult> ShowAsync() => _contentDialog.ShowAsync();
    public static IAsyncOperation<ContentDialogResult> ShowWithInfoAsync(ContentDialogInfo dialogInfo, XamlRoot xamlRoot)
    {
        ContentDialogService service = new(dialogInfo, xamlRoot);
        return service.ShowAsync();
    }

    public static void SetDialogKind(ContentDialog dialog, ContentDialogKind kind)
    {
        dialog.Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"];

        if (kind == ContentDialogKind.Close)
        {
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.CloseButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
            dialog.CloseButtonText = "Close/Text".GetLocalized();
        }
        else if (kind == ContentDialogKind.OkClose)
        {
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
            dialog.PrimaryButtonText = "Ok/Text".GetLocalized();
            dialog.CloseButtonText = "Close/Text".GetLocalized();
        }
        else if (kind == ContentDialogKind.SaveUnsaveCancel)
        {
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"];
            dialog.PrimaryButtonText = "Save/Text".GetLocalized();
            dialog.SecondaryButtonText = "Unsave/Text".GetLocalized();
            dialog.CloseButtonText = "Cancel/Text".GetLocalized();
        }
    }

    public static ContentDialog TryGetOpenedContentDialog()
    {
        var popups = VisualTreeHelper.GetOpenPopupsForXamlRoot(App.Window.Content.XamlRoot);

        foreach (var popup in popups)
        {
            if (popup.Child is ContentDialog dialog)
            {
                return dialog;
            }
        }

        return null;
    }
}
