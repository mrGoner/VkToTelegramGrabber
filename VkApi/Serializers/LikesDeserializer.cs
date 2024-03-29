﻿using System;
using System.Text.Json;

namespace VkApi.Serializers
{
    public class LikesDeserializer
    {
        public int ParseLikesCount(string _data)
        {
            if (string.IsNullOrWhiteSpace(_data))
                throw new ArgumentException("data can not be null or empty", nameof(_data));

            try
            {
               return JsonDocument.Parse(_data).RootElement.GetProperty("response").GetProperty("likes").GetInt32();
            }
            catch(Exception ex)
            {
                throw new DeserializerException("Failed to parse likes count", _data, ex);
            }
        } 
    }
}
