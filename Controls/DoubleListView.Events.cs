using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace MyDairy.Controls;

public partial class DoubleListView
{
    public event SelectionChangedEventHandler SelectionChanged;

    public event SelectionChangedEventHandler InnerSelectionChanged;
}
