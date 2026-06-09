using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Serialization;

namespace MyDairy.Models;

public partial class Note : ObservableObject
{
    private DateTime _lastEditTime;
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime LastEditTime
    {
        get => _lastEditTime;
        set => SetProperty(ref _lastEditTime, value);
    }

    private string _content;
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public Note()
    {
        
    }

    public Note(string content, DateTime lastEditTime)
    {
        Content = content;
        LastEditTime = lastEditTime;
    }
}