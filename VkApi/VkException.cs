using System;

namespace VkApi
{
    public class VkException : Exception
    {
        public VkException(string _message) : base(_message)
        {

        }

        public VkException(string _message, Exception _innerException) : base(_message, _innerException)
        {

        }
    }
}
