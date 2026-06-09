using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyDairy.Common;

namespace MyDairy.Serialization;

public class CustomTimeOnlyConverter : JsonConverter<TimeOnly?>
{
    public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is string timeOnly)
        {
            return TimeOnly.ParseExact(timeOnly, GlobalConstants.TimeOnlyFormat);
        }
        return null;
    }
    public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(GlobalConstants.TimeOnlyFormat));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
