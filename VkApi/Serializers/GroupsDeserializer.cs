using System;
using System.Text.Json;
using VkApi.ObjectModel;

namespace VkApi.Serializers
{
    public class GroupsDeserializer
    {
        public Groups Deserialize(string _data)
        {
            try
            {
                using var document = JsonDocument.Parse(_data);

               return new Groups(document.RootElement.GetProperty("response").GetProperty("items").Deserialize<Group[]>());
            }
            catch (Exception ex)
            {
                throw new DeserializerException("Failed to deserialize groups", _data, ex);
            }
        }
    }
}
