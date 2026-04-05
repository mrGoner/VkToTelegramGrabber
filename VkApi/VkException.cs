using System;

namespace VkApi;

public class VkException : Exception
{
    public VkException(string message) : base(message)
    {
    }

    public VkException(string message, Exception innerException) : base(message, innerException)
    {
    }
}