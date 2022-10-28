using System.Text.Json.Serialization;

namespace VkApi.ObjectModel
{
    public struct Views
    {
        [JsonPropertyName("count")]
        public int Count { get; }
    }
}