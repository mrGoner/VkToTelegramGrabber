using System;

namespace VkTools.Serializers
{
    public class NewsFeedSerializerException : Exception
    {
        public string ErrorObject;

        public NewsFeedSerializerException(string _message) : base(_message)
        {

        }

        public NewsFeedSerializerException(string _message, Exception _ex) : base(_message, _ex)
        {

        }

        public NewsFeedSerializerException(string _message, string _errorObj) : base(_message)
        {

        }
    }
}
