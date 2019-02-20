using System;

namespace VkTools.Serializers
{
    public class NewsFeedDeserializerException : Exception
    {
        public string ErrorObject;

        public NewsFeedDeserializerException(string _message) : base(_message)
        {

        }

        public NewsFeedDeserializerException(string _message, Exception _ex) : base(_message, _ex)
        {

        }

        public NewsFeedDeserializerException(string _message, string _errorObj) : base(_message)
        {

        }
    }
}
