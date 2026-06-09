using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace MyDairy.Controls;

public partial class DataPresenter : Control
{
    public DataPresenter()
    {
        DefaultStyleKey = typeof(DataPresenter);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        OnDataChanged();
    }

    private void OnDataChanged()
    {
        if (Data != null)
        {
            VisualStateManager.GoToState(this, "HasData", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "NoData", true);
        }
    }

    public void ShowExtraContent()
    {
        VisualStateManager.GoToState(this, "Extra", true);
    }

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(DataPresenter), new(null, (d, e) => ((DataPresenter)d).OnDataChanged()));

    public DataTemplate DataTemplate
    {
        get => (DataTemplate)GetValue(DataTemplateProperty);
        set => SetValue(DataTemplateProperty, value);
    }

    public static readonly DependencyProperty DataTemplateProperty = DependencyProperty.Register(nameof(DataTemplate), typeof(DataTemplate), typeof(DataPresenter), new(null));

    public object NoDataContent
    {
        get => GetValue(NoDataContentProperty);
        set => SetValue(NoDataContentProperty, value);
    }

    public static readonly DependencyProperty NoDataContentProperty = DependencyProperty.Register(nameof(NoDataContent), typeof(object), typeof(DataPresenter), new(null));

    public object ExtraContent
    {
        get => GetValue(ExtraContentProperty);
        set => SetValue(ExtraContentProperty, value);
    }

    public static readonly DependencyProperty ExtraContentProperty = DependencyProperty.Register(nameof(ExtraContent), typeof(object), typeof(DataPresenter), new(null));

}
