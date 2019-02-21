using System;
using VkApi.Requests;
using VkTools.Serializers;
using VkTools.ObjectModel.Wall;

namespace VkApi
{
    public class Vk
    {
        private readonly RequestExecutor m_requestExecutor;
        private readonly NewsFeedDeserializer m_newsFeedDeserializer;
        private readonly GroupsDeserializer m_groupsDeserializer;
        private readonly string m_currentVkVersion;
        private const string m_baseUrl = "https://api.vk.com/method";

        public Vk(string _version)
        {
            if (string.IsNullOrWhiteSpace(_version))
                throw new ArgumentException("Version not recognized as valid vk version!");

            m_currentVkVersion = _version;
            m_requestExecutor = new RequestExecutor(m_baseUrl);
            m_newsFeedDeserializer = new NewsFeedDeserializer();
            m_groupsDeserializer = new GroupsDeserializer();
        }

        public NewsFeed GetNewsFeed(string _userToken, DateTime _start, DateTime _end, string _sourceIds)
        {
            var request = RequestBuilder.BuildNewsFeedRequest(_userToken, m_currentVkVersion,
                             _start.ToUniversalTime(), _end.ToUniversalTime(), _sourceIds);
            var responseData = m_requestExecutor.Execute(request);

            var newsFeed = m_newsFeedDeserializer.Deserialize(responseData);

            return newsFeed;
        }
    }
}
