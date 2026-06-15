using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using MyDairy.Common;
using MyDairy.Models;
using MyDairy.Serialization;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace MyDairy.Helpers;

public static class ComponentHelper
{
    public static void OpenSettingsColor()
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "ms-settings:colors",
            UseShellExecute = true,
        });
    }

    public static DateTimeOffset ToOffset(this DateOnly date)
    {
        return new(date, TimeOnly.MinValue, TimeSpan.Zero);
    }

    public static SizeInt32 ToSizeInt32(this Size size)
    {
        return new((int)size.Width, (int)size.Height);
    }

    public static unsafe void SetWindowIcon(Window window)
    {
        HWND hWnd = new((nint)window.AppWindow.Id.Value);
        var hInstance = PInvoke.GetModuleHandle((PCWSTR)null);
        var bigIcon = PInvoke.LoadImage(hInstance, (char*)32512, GDI_IMAGE_TYPE.IMAGE_ICON, 32, 32, IMAGE_FLAGS.LR_DEFAULTCOLOR);
        var smallIcon = PInvoke.LoadImage(hInstance, (char*)32512, GDI_IMAGE_TYPE.IMAGE_ICON, 16, 16, IMAGE_FLAGS.LR_DEFAULTCOLOR);

        PInvoke.SendMessage(hWnd, PInvoke.WM_SETICON, PInvoke.ICON_SMALL, (nint)smallIcon.Value);
        PInvoke.SendMessage(hWnd, PInvoke.WM_SETICON, PInvoke.ICON_BIG, (nint)bigIcon.Value);
    }
}
