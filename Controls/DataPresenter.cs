using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT;

namespace MyDairy.Controls;

public partial class DataPresenter : Control
{
    private ContentPresenter _extraContent;
    private Storyboard _storyboard;

    public DataPresenter()
    {
        DefaultStyleKey = typeof(DataPresenter);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _extraContent = GetTemplateChild("ExtraPresenter").As<ContentPresenter>();
        _storyboard = GetTemplateChild("ExtraStoryboard").As<Storyboard>();
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

    public void ShowExtraContent(int index)
    {
        _extraContent.Content = ExtraContents[index];
        VisualStateManager.GoToState(this, "Extra", true);
        _storyboard?.Begin();
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

    public IList<object> ExtraContents
    {
        get;
    } = [];
}
