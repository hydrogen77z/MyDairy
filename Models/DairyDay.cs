using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Serialization;
using MyDairy.Settings;

namespace MyDairy.Models;

public partial class DairyDay : ObservableObject
{
    [JsonConverter(typeof(CustomDateOnlyConverter))]
    public DateOnly Date
    {
        get; set;
    }

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<DairyText> Texts
    {
        get;
    } = [];

    [JsonIgnore]
    public string CountString => AppSettings.Instance.ShowDairyCount ? $"({Texts.Count})" : string.Empty;

    public override string ToString() => XamlHelper.DateOnlyToString(Date);

    public DairyDay()
    {
        Texts.CollectionChanged += Texts_CollectionChanged;
    }

    private void Texts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var text in e.NewItems)
            {
                ((DairyText)text).SetSource(Date);
            }
        }

        DairyText.NoticeFileIsUpdated();
        UpdateCountString();
    }

    internal void UpdateCountString()
    {
        OnPropertyChanged(nameof(CountString));
    }
}
