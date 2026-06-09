using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MyDairy.Common;
using MyDairy.Models;

namespace MyDairy.Controls;

public partial class DairyTreeViewItemSelector : DataTemplateSelector
{
    public DataTemplate DairyDayTemplate
    {
        get; set;
    }
    public DataTemplate DairyTextTemplate
    {
        get; set;
    }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is DairyDay)
        {
            return DairyDayTemplate;
        }
        else if (item is DairyText)
        {
            return DairyTextTemplate;
        }
        else
        {
            return null;
        }
    }
}
