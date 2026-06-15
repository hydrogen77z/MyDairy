using System;
using System.Collections.Generic;
using System.Text;

namespace MyDairy.Common;

public interface ISupportContainedInWindow
{
    void SetContainToWindow(bool inWindow);

    bool GetIsBackButtonVisible();
    void OnBackRequested();

    string Title
    {
        get;
    }

    event EventHandler TitleChanged;
}
