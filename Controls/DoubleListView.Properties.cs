using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using MyDairy.Common;

namespace MyDairy.Controls;

public partial class DoubleListView
{
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DoubleListView), new(null));

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DoubleListView), new(null));

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DoubleListView), new(null, (d, e) => ((DoubleListView)d).OnSelectedItemChanged()));

    public object InnerItemsSource
    {
        get => GetValue(InnerItemsSourceProperty);
        set => SetValue(InnerItemsSourceProperty, value);
    }

    public static readonly DependencyProperty InnerItemsSourceProperty = DependencyProperty.Register(nameof(InnerItemsSource), typeof(object), typeof(DoubleListView), new(null));

    public DataTemplate InnerItemTemplate
    {
        get => (DataTemplate)GetValue(InnerItemTemplateProperty);
        set => SetValue(InnerItemTemplateProperty, value);
    }

    public static readonly DependencyProperty InnerItemTemplateProperty = DependencyProperty.Register(nameof(InnerItemTemplate), typeof(DataTemplate), typeof(DoubleListView), new(null));

    public object InnerSelectedItem
    {
        get => GetValue(InnerSelectedItemProperty);
        set => SetValue(InnerSelectedItemProperty, value);
    }

    public static readonly DependencyProperty InnerSelectedItemProperty = DependencyProperty.Register(nameof(InnerSelectedItem), typeof(object), typeof(DoubleListView), new(null, (d, e) => ((DoubleListView)d).OnInnerSelectedItemChanged()));

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }

    public static readonly DependencyProperty InnerContentProperty = DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(DoubleListView), new(null));

    public DataTemplate InnerContentTemplate
    {
        get => (DataTemplate)GetValue(InnerContentTemplateProperty);
        set => SetValue(InnerContentTemplateProperty, value);
    }

    public static readonly DependencyProperty InnerContentTemplateProperty = DependencyProperty.Register(nameof(InnerContentTemplate), typeof(DataTemplate), typeof(DoubleListView), new(null));

    public object InnerBottomContent
    {
        get => (object)GetValue(InnerBottomContentProperty);
        set => SetValue(InnerBottomContentProperty, value);
    }

    public static readonly DependencyProperty InnerBottomContentProperty = DependencyProperty.Register(nameof(InnerBottomContent), typeof(object), typeof(DoubleListView), new(null));

    public object NoInnerContent
    {
        get => GetValue(NoInnerContentProperty);
        set => SetValue(NoInnerContentProperty, value);
    }

    public static readonly DependencyProperty NoInnerContentProperty = DependencyProperty.Register(nameof(NoInnerContent), typeof(object), typeof(DoubleListView), new(null));

    public DoubleListViewDisplayMode DisplayMode
    {
        get => (DoubleListViewDisplayMode)GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(DoubleListViewDisplayMode), typeof(DoubleListView), new(DoubleListViewDisplayMode.Split, (d, e) => ((DoubleListView)d).OnDisplayModeChanged()));

    public GridLength ListHeight
    {
        get => (GridLength)GetValue(ListHeightProperty);
        set => SetValue(ListHeightProperty, value);
    }

    public static readonly DependencyProperty ListHeightProperty = DependencyProperty.Register(nameof(ListHeight), typeof(GridLength), typeof(DoubleListView), new(new GridLength(1, GridUnitType.Star)));

    public GridLength InnerListHeight
    {
        get => (GridLength)GetValue(InnerListHeightProperty);
        set => SetValue(InnerListHeightProperty, value);
    }

    public static readonly DependencyProperty InnerListHeightProperty = DependencyProperty.Register(nameof(InnerListHeight), typeof(GridLength), typeof(DoubleListView), new(new GridLength(1, GridUnitType.Star)));
}
