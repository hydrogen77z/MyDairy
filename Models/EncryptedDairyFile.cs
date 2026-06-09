using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using MyDairy.Serialization;

namespace MyDairy.Models;

public partial class EncryptedDairyFile
{
    [JsonConverter(typeof(CustomBase64ByteArrayConverter))]
    public byte[] Salt
    {
        get; set;
    }

    [JsonConverter(typeof(CustomBase64ByteArrayConverter))]
    public byte[] Nonce
    {
        get; set;
    }

    [JsonConverter(typeof(CustomBase64ByteArrayConverter))]
    public byte[] Tag
    {
        get; set;
    }

    public string EncryptedContent
    {
        get; set;
    }

    public EncryptedDairyFile()
    {
    }
}
