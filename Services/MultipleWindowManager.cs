using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.UI.Xaml;
using MyDairy.Views;
using WinRT;

namespace MyDairy.Services;

public class MultipleWindowManager
{
    public readonly IList<ContentWindow> Windows = [];

    public void CreateWindowWithConfiguration(ContentWindow window)
    {
        Windows.Add(window);
        window.Closed += OnWindowClosed;

        window.Activate();
    }

    public void CloseAll()
    {
        for (var i = Windows.Count - 1; i >= 0; i--)
        {
            Windows[i].Close();
        }
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        Windows.Remove(sender.As<ContentWindow>());
    }
}
