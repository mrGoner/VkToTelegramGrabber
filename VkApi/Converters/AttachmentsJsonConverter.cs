using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using VkApi.Extensions;
using VkApi.ObjectModel.Attachments;
using VkApi.ObjectModel.Attachments.Audio;
using VkApi.ObjectModel.Attachments.Doc;
using VkApi.ObjectModel.Attachments.Link;
using VkApi.ObjectModel.Attachments.Photo;
using VkApi.ObjectModel.Attachments.Video;

namespace VkApi.Converters;

public class AttachmentsJsonConverter : JsonConverter<IAttachmentElement[]>
{
    public override IAttachmentElement[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.StartArray)
         return Array.Empty<IAttachmentElement>();

        var attachments = new List<IAttachmentElement>();

        while (reader.Read())
        {
            if(reader.TokenType == JsonTokenType.EndArray)
                break;
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
        while (reader.Read())
        {
            if(reader.TokenType == JsonTokenType.EndObject)
                return null;

            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "type")
            {
                reader.Read();

                var attachmentType = reader.GetString();

                reader.ReadToNextObject();
               
                switch (attachmentType)
                {
                    case "video":
                        return JsonSerializer.Deserialize<VideoAttachment>(ref reader);
                    case "audio":
                        return JsonSerializer.Deserialize<AudioAttachment>(ref reader);
                    case "link":
                        return JsonSerializer.Deserialize<LinkAttachment>(ref reader);
                    case "doc":
                        return JsonSerializer.Deserialize<DocumentAttachment>(ref reader);
                    case "photo":
                        return JsonSerializer.Deserialize<PhotoAttachment>(ref reader);
                    default:
                        return new UnsupportedAttachment(attachmentType);
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