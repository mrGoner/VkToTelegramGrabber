using System.Text.Json;
using VkApi.ObjectModel;
using System;
using System.Linq;

namespace VkApi.Serializers
{
    public class VideoInfoDeserializer
    {
        public VideoInfo Deserialize(string _data)
        {
            try
            {
                using var document = JsonDocument.Parse(_data);
                var items = document.RootElement.GetProperty("response").GetProperty("items");

                return items.Deserialize<VideoInfo[]>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new DeserializerException("Failed to deserialize video info", _data, ex);
            }
        }
    }
}
