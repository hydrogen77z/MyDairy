using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MyDairy.Common;
using MyDairy.Helpers;

namespace MyDairy.Attached;

public static class DialogExtension
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.RegisterAttached("Kind", typeof(ContentDialogKind?), typeof(DialogExtension), new(null, OnKindChanged));

    public static ContentDialogKind? GetKind(ContentDialog dialog)
    {
        return (ContentDialogKind?)dialog.GetValue(KindProperty);
    }
    public static void SetKind(ContentDialog dialog, ContentDialogKind? kind)
    {
        dialog.SetValue(KindProperty, kind);
    }

    public static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ContentDialog dialog)
        {
            return;
        }

        if (GetKind(dialog) is not ContentDialogKind kind)
        {
            return;
        }

        dialog.Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"];
        dialog.XamlRoot = App.Window.Content.XamlRoot;

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
