using System.Text.Json;

namespace VkApi.Extensions;

public static class ReaderExtensions
{
    public static Utf8JsonReader ReadToNextObject(this ref Utf8JsonReader reader)
    {
        do
        {
            if(reader.TokenType == JsonTokenType.StartObject)
                break;

        }while(reader.Read());

        return reader;
    }
}