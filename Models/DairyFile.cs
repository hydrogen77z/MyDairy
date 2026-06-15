using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using MyDairy.Helpers;
using MyDairy.Serialization;

namespace MyDairy.Models;

public partial class DairyFile
{
    public string Name
    {
        get; set;
    } = string.Empty;

    public string Author
    {
        get; set;
    } = string.Empty;

    public string Description
    {
        get; set;
    } = string.Empty;

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime CreateTime
    {
        get; set;
    }

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime LastEditTime
    {
        get; set;
    }

    //[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<DairyDay> DairyDays
    {
        get;
    }

    public DairyDay GetDayFromText(DairyText text)
    {
        if (text is null)
        {
            return null;
        }

        for (var i = 1; i <= DairyDays.Count; i++)
        {
            if (DairyDays[^i].Date == text.SourceDate)
            {
                return DairyDays[^i];
            }
        }

        return null;
    }

    [JsonConstructor]
    public DairyFile(string name, string author, string description, DateTime createTime, DateTime lastEditTime, ObservableCollection<DairyDay> dairyDays)
    {
        Name = name;
        Author = author;
        Description = description;
        CreateTime = createTime;
        LastEditTime = lastEditTime;

        DairyDays = new(dairyDays.OrderBy(d => d.Date));
    }

    public DairyFile()
    {
    }
}
