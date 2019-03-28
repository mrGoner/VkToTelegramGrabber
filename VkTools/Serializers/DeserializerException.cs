using System;

namespace VkTools.Serializers
{
    public class DeserializerException : Exception
    {
        public DeserializerException(string _message) : base(_message)
        {

        }

        public DeserializerException(string _message, Exception _ex) : base(_message, _ex)
        {

        }
    }
}
