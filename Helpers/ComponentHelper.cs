using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Models;
using MyDairy.Serialization;

namespace MyDairy.Helpers;

public static class ComponentHelper
{
    public static void OpenSettingsColor()
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "ms-settings:colors",
            UseShellExecute = true,
        });
    }

    public static DateTimeOffset ToOffset(this DateOnly date)
    {
        return new(date, TimeOnly.MinValue, TimeSpan.Zero);
    }
}
