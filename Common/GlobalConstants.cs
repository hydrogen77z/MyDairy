using System;
using System.Collections.Generic;
using System.Text;

namespace MyDairy.Common;

public static class GlobalConstants
{
    public const string DateOnlyFormat = "yyyy/MM/dd";
    public const string TimeOnlyFormat = "HH:mm:ss";
    public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

    public const string DateAndTimeNullFormat = "{0:yyyy/MM/dd} --:--:--";

    public const string EncryptedContentKey = "encrypted_content";

    public static readonly Version CurrentVersion = new(1, 0, 0);

    public const char BlackCircle = '\u25CF';
}
