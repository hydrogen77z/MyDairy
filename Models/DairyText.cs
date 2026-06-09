using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Helpers;
using MyDairy.Serialization;
using MyDairy.Views;

namespace MyDairy.Models;

public partial class DairyText : ObservableObject
{
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
            {
                IsSaved = false;
            }
        }
    }

    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            if (SetProperty(ref _description, value))
            {
                IsSaved = false;
            }
        }
    }

    private string _body;
    public string Body
    {
        get => _body;
        set
        {
            if (SetProperty(ref _body, value))
            {
                IsSaved = false;
            }
        }
    }

    private bool _isTimeApproximate = false;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsTimeApproximate
    {
        get => _isTimeApproximate;
        set
        {
            if (SetProperty(ref _isTimeApproximate, value))
            {
                IsSaved = false;
            }
        }
    }

    [JsonConverter(typeof(CustomTimeOnlyConverter))]
    public TimeOnly? CreateTime
    {
        get;
        set;
    }

    private DateTime _lastEditTime;
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime LastEditTime
    {
        get => _lastEditTime;
        set => SetProperty(ref _lastEditTime, value);
    }

    private bool _isSaved = true;
    [JsonIgnore]
    public bool IsSaved
    {
        get => _isSaved;
        set
        {
            if (SetProperty(ref _isSaved, value))
            {
                if (!_isSaved)
                {
                    NoticeFileIsUpdated();
                }
            }
        }
    }

    [JsonIgnore]
    public DateOnly SourceDate
    {
        get;
        private set;
    }

    //public string DisplayCreateTime => XamlHelper.DateAndTimeToString(SourceDate, CreateTime);

    internal static void NoticeFileIsUpdated()
    {
        DairyPage.Instance.ViewModel.NoticeFileEdited();
    }

    internal void SetSource(DateOnly sourceDate)
    {
        SourceDate = sourceDate;
        OnPropertyChanged(nameof(SourceDate));
    }

    internal void OnFileSaved()
    {
        if (_isSaved == false)
        {
            LastEditTime = DateTime.Now;
            IsSaved = true;
        }
    }
    // To do: JsonConstructor
    [JsonConstructor]
    public DairyText(string title, string description, string body, bool isTimeApproximate, TimeOnly? createTime, DateTime lastEditTime)
    {
        _title = title;
        _description = description;
        _body = body;
        _isTimeApproximate = isTimeApproximate;
        CreateTime = createTime;
        _lastEditTime = lastEditTime;
    }

    public DairyText()
    {
    }
}
