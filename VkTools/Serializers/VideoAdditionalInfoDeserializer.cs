using System;
using Newtonsoft.Json.Linq;

namespace VkTools.Serializers
{
    internal class VideoAttachmentDeserializer
    {
        public VideoAdditionalInfo Deserialize(string _data)
        {
            var jObject = JObject.Parse(_data);

            if (jObject["response"]["items"] is JArray jItems)
            {
                try
                {
                    if(jItems.Count > 0)
                    {
                        if (jItems[0] is JObject jItem)
                        {
                            if (jItem.ContainsKey("player"))
                            {
                                return new VideoAdditionalInfo
                                {
                                    PlayerUrl = jItem["player"].Value<string>()
                                };
                            }
                        }
                    }

                    return new VideoAdditionalInfo();
                }
                catch (Exception ex)
                {
                    throw new DeserializerException("Failed to deserialize video additional info", ex);
                }
            }

            throw new DeserializerException($"Failed recognize jObject as vk video item \n {_data}");
        }
    }

    public class VideoAdditionalInfo
    {
        public string PlayerUrl { get; set; }
    }
}
