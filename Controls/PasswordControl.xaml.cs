using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MyDairy.Common;
using MyDairy.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyDairy.Controls;

public sealed partial class PasswordControl : UserControl
{
    public PasswordInputMode PasswordInputMode
    {
        get => (PasswordInputMode)GetValue(PasswordInputModeProperty);
        set => SetValue(PasswordInputModeProperty, value);
    }

    public static readonly DependencyProperty PasswordInputModeProperty = DependencyProperty.Register(nameof(PasswordInputMode), typeof(PasswordInputMode), typeof(PasswordControl), new(PasswordInputMode.Request, (d,e) => ((PasswordControl)d).OnPasswordInputModeChanged()));

    public bool IsPasswordValid
    {
        get => (bool)GetValue(IsPasswordValidProperty);
        set => SetValue(IsPasswordValidProperty, value);
    }

    public static readonly DependencyProperty IsPasswordValidProperty = DependencyProperty.Register(nameof(IsPasswordValid), typeof(bool), typeof(PasswordControl), new(false));

    private bool _requestValid = false;
    private bool _verifyValid = false;
    public PasswordControl()
    {
        InitializeComponent();

        OnPasswordInputModeChanged();
    }

    private void OnPasswordInputModeChanged()
    {
        RequestBox.PasswordChanged -= RequestBox_PasswordChanged;
        ChangeBox.PasswordChanged -= VerifyBox_PasswordChanged;
        VerifyBox.PasswordChanged -= VerifyBox_PasswordChanged;
        ClearPassword();

        switch (PasswordInputMode)
        {
            case PasswordInputMode.Request:
                _verifyValid = true;
                RequestBox_PasswordChanged(null, null);
                RequestBox.PasswordChanged += RequestBox_PasswordChanged;
                VisualStateManager.GoToState(this, "Request", false);
                break;
            case PasswordInputMode.Create:
                _requestValid = true;
                VerifyBox_PasswordChanged(null, null);
                ChangeBox.PasswordChanged += VerifyBox_PasswordChanged;
                VerifyBox.PasswordChanged += VerifyBox_PasswordChanged;
                VisualStateManager.GoToState(this, "Create", false);
                break;
            case PasswordInputMode.Change:
                RequestBox_PasswordChanged(null, null);
                VerifyBox_PasswordChanged(null, null);
                RequestBox.PasswordChanged += RequestBox_PasswordChanged;
                ChangeBox.PasswordChanged += VerifyBox_PasswordChanged;
                VerifyBox.PasswordChanged += VerifyBox_PasswordChanged;
                VisualStateManager.GoToState(this, "Change", false);
                break;
            default:
                return;
        }
    }

    private void UpdateIsValid()
    {
        IsPasswordValid = _requestValid && _verifyValid;
    }

    private void RequestBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _requestValid = !string.IsNullOrEmpty(RequestBox.Password);
        RequestPromptPart.Visibility = XamlHelper.ToCollapsed(_requestValid);

        UpdateIsValid();
    }

    private void VerifyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _verifyValid = !string.IsNullOrEmpty(VerifyBox.Password) && VerifyBox.Password.Equals(ChangeBox.Password);
        VerifyPromptPart.Visibility = XamlHelper.ToCollapsed(_verifyValid);

        UpdateIsValid();
    }

    public void ClearPassword()
    {
        RequestBox.Password = null;
        ChangeBox.Password = null;
        VerifyBox.Password = null;

        ShowIncorrectPrompt(true);
    }

    public bool TryGetRequestPassword(out string requestPassword)
    {
        if (PasswordInputMode == PasswordInputMode.Request || PasswordInputMode == PasswordInputMode.Change)
        {
            requestPassword = RequestBox.Password;
            return true;
        }

        requestPassword = null;
        return false;
    }

    public bool TryGetVerifyPassword(out string verifyPassword)
    {
        if (PasswordInputMode == PasswordInputMode.Create || PasswordInputMode == PasswordInputMode.Change)
        {
            verifyPassword = VerifyBox.Password;
            return true;
        }

        verifyPassword = null;
        return false;
    }

    public void ShowIncorrectPrompt(bool succeed)
    {
        IncorrectPromptPart.Visibility = XamlHelper.ToCollapsed(succeed);
    }
}
