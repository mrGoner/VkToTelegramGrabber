using System;

namespace VkTools.Serializers
{
    public class DeserializerException : Exception
    {
        public string ErrorObject { get; }

        public DeserializerException(string _message) : base(_message)
        {

        }

        public DeserializerException(string _message, Exception _ex) : base(_message, _ex)
        {

        }

        public DeserializerException(string _message, string _errorObj) : base(_message)
        {
            ErrorObject = _errorObj;
        }

        public DeserializerException(string _message, string _errorObj, Exception _innerException) : base(_message, _innerException)
        {
            ErrorObject = _errorObj;
        }
    }
}
