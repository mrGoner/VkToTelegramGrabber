using System;

namespace VkTools.Serializers
{
    public class DeserializerException : Exception
    {
        public string ErrorObject;

        public DeserializerException(string _message) : base(_message)
        {

        }

        public DeserializerException(string _message, Exception _ex) : base(_message, _ex)
        {

        }

        public DeserializerException(string _message, string _errorObj) : base(_message)
        {

        }
    }
}
