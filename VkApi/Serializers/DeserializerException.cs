using System;

namespace VkApi.Serializers;

public class DeserializerException : Exception
{
    public DeserializerException(string message, string data) : base($"{message} data: {data}")
    {
    }

    public DeserializerException(string message, string data, Exception ex) : base($"{message} data: {data}", ex)
    {
    }
}