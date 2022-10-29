using System;

namespace VkApi.Serializers
{
    public class DeserializerException : Exception
    {
        public DeserializerException(string _message, string _data) : base($"{_message} data: {_data}")
        {

        }

        public DeserializerException(string _message, string _data, Exception _ex) : base($"{_message} data: {_data}", _ex)
        {

        }
    }
}
