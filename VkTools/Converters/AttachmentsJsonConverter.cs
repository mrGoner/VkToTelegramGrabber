using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using VkTools.ObjectModel.Attachments;
using VkTools.ObjectModel.Attachments.Audio;
using VkTools.ObjectModel.Attachments.Doc;
using VkTools.ObjectModel.Attachments.Link;
using VkTools.ObjectModel.Attachments.Photo;
using VkTools.ObjectModel.Attachments.Video;

namespace VkTools.Converters;

public class AttachmentsJsonConverter : JsonConverter<IAttachmentElement[]>
{
    public override IAttachmentElement[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var attachments = new List<IAttachmentElement>();

        while (reader.Read() || reader.TokenType != JsonTokenType.EndArray)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var parsedAttachment = ParseAttachment(ref reader);
                    if (parsedAttachment != null)
                        attachments.Add(parsedAttachment);
                    break;
            }
        }
        
        return attachments.ToArray();
    }

    private IAttachmentElement ParseAttachment(ref Utf8JsonReader reader)
    {
        var readerCopy = reader;
        
        while (reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "type")
            {
                reader.Read();

                switch (reader.GetString())
                {
                    case "video":
                        return JsonSerializer.Deserialize<VideoAttachment>(ref readerCopy);
                    case "audio":
                        return JsonSerializer.Deserialize<AudioAttachment>(ref readerCopy);
                    case "link":
                        return JsonSerializer.Deserialize<LinkAttachment>(ref readerCopy);
                    case "doc":
                        return JsonSerializer.Deserialize<DocumentAttachment>(ref readerCopy);
                    case "image":
                        return JsonSerializer.Deserialize<PhotoAttachment>(ref readerCopy);
                    default:
                        return new UnsupportedAttachment(reader.GetString());
                }
            }
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, IAttachmentElement[] value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}