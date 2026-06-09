using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MyDairy.Common;
using WinRT;

namespace MyDairy.Controls;

public partial class DoubleListView : Control
{
    private TwoPaneView _twoPaneView = null;
    private ListView _rootListView = null;
    private ListView _innerListView = null;
    private Button _backButton = null;

    private DiscreteDoubleKeyFrame _selected = null;
    private DiscreteDoubleKeyFrame _back = null;

    public DoubleListView()
    {
        DefaultStyleKey = typeof(DoubleListView);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _twoPaneView = GetTemplateChild("TwoPaneView").As<TwoPaneView>();
        _rootListView = GetTemplateChild("RootListView").As<ListView>();
        _innerListView = GetTemplateChild("InnerListView").As<ListView>();
        _backButton = GetTemplateChild("BackButton").As<Button>();
        _selected = GetTemplateChild("CompactSelectedAnimation").As<DiscreteDoubleKeyFrame>();
        _back = GetTemplateChild("CompactBackAnimation").As<DiscreteDoubleKeyFrame>();

        _backButton.Click += BackButton_Click;
        _rootListView.SelectionChanged += RootListView_SelectionChanged;
        _innerListView.SelectionChanged += InnerListView_SelectionChanged;

        OnDisplayModeChanged();
        UpdateInnerContent();
    }

    #region Events
    private void UpdateInnerContent()
    {
        if (SelectedItem != null)
        {
            _selected.Value = ActualWidth;
        }
        else
        {
            _back.Value = -ActualWidth;
        }

        if (DisplayMode == DoubleListViewDisplayMode.Compact)
        {
            _twoPaneView.PanePriority = SelectedItem != null ? TwoPaneViewPriority.Pane2 : TwoPaneViewPriority.Pane1;

            VisualStateManager.GoToState(this, SelectedItem != null ? "CompactToInner" : "CompactExitInner", true);
        }
        else
        {
            VisualStateManager.GoToState(this, SelectedItem != null ? "HasInner" : "NoInner", true);
        }
    }

    private void OnSelectedItemChanged()
    {
        _rootListView.SelectedItem = SelectedItem;
    }

    private void OnInnerSelectedItemChanged()
    {
        _innerListView.SelectedItem = InnerSelectedItem;
    }

    private void RootListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItem = ((ListView)sender).SelectedItem;

        UpdateInnerContent();
        SelectionChanged?.Invoke(this, new([], [SelectedItem]));
    }

    private void InnerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        InnerSelectedItem = ((ListView)sender).SelectedItem;

        UpdateInnerContent();
        InnerSelectionChanged?.Invoke(this, new([], [InnerSelectedItem]));
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (DisplayMode == DoubleListViewDisplayMode.Compact)
        {
            _rootListView.SelectedItem = null;
        }
    }

    private void OnDisplayModeChanged()
    {
        if (_twoPaneView is null)
        {
            return;
        }

        if (DisplayMode == DoubleListViewDisplayMode.Compact)
        {
            //_twoPaneView.MinTallModeHeight = double.PositiveInfinity;
            //_backButton.Visibility = Visibility.Visible;
            VisualStateManager.GoToState(this, "Compact", true);
        }
        else if (DisplayMode == DoubleListViewDisplayMode.Split)
        {
            //_twoPaneView.MinTallModeHeight = 0;
            //_backButton.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "Normal", true);
        }
    }
    #endregion
}
