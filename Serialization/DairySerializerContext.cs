using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyDairy.Helpers;
using MyDairy.Models;

namespace MyDairy.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    IncludeFields = true,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(DairyFile))]
internal partial class DairySerializerContext : JsonSerializerContext
{

}
