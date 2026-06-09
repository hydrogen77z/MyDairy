using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Models;
using MyDairy.Settings;

namespace MyDairy.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    IndentSize = 4,
    IncludeFields = true,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(EncryptedDairyFile))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
