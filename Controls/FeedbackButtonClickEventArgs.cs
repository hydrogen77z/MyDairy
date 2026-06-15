using System;
using System.Collections.Generic;
using System.Text;

namespace MyDairy.Controls;

public partial class FeedbackButtonClickEventArgs : EventArgs
{
    public bool BeginStoryboard
    {
        get;
        set;
    } = true;
}
