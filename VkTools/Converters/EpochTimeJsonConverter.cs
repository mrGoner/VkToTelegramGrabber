using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VkTools.Converters;

public class EpochTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
       return EpochTimeConverter.ConvertToDateTime(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}