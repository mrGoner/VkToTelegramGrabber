using System;
using VkApi.Requests;
using VkTools.Serializers;
using VkTools.ObjectModel.Wall;
using VkTools.ObjectModel;

namespace VkApi
{
    public class Vk
    {
        private readonly RequestExecutor m_requestExecutor;
        private readonly NewsFeedDeserializer m_newsFeedDeserializer;
        private readonly GroupsDeserializer m_groupsDeserializer;
        private readonly LikesDeserializer m_likesDeserializer;
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
            m_likesDeserializer = new LikesDeserializer();
        }

        public NewsFeed GetNewsFeed(string _userToken, DateTime _start, DateTime _end, string _sourceIds)
        {
            try
            {
                var request = RequestBuilder.BuildNewsFeedRequest(_userToken, m_currentVkVersion,
                                 _start, _end, _sourceIds);

                var responseData = m_requestExecutor.Execute(request);

                var getVideoFunc = new Func<VideoInfo, string>((_arg) =>
                {
                    try
                    {
                        var getVideoRequest = RequestBuilder.BuildGetVideoRequest(_userToken, m_currentVkVersion, _arg.OwnerId, _arg.VideoId);

                        var videoResponseData = m_requestExecutor.Execute(getVideoRequest);

                        return videoResponseData;
                    }
                    catch (Exception ex)
                    {
                        throw new VkException("Failed to get video", ex);
                    }
                });

                var newsFeed = m_newsFeedDeserializer.Deserialize(responseData, getVideoFunc);

                return newsFeed;
            }
            catch (Exception ex)
            {
                throw new VkException("Failed to get newsfeed", ex);
            }
        }

        public Groups GetGroups(string _userToken, int _count)
        {
            try
            {
                var request = RequestBuilder.BuildGroupRequest(_userToken, m_currentVkVersion, _count);

                var responseData = m_requestExecutor.Execute(request);

                var groups = m_groupsDeserializer.Deserialize(responseData);

                return groups;
            }
            catch (Exception ex)
            {
                throw new VkException("Failed to get groups", ex);
            }
        }

        public int LikePost(int _itemOwner, uint _itemId, string _userToken)
        {
            try
            {
                var request = RequestBuilder.BuildLikeRequest(LikeType.Post, _itemOwner, _itemId, _userToken, m_currentVkVersion);

                var responseData = m_requestExecutor.Execute(request);

                var likesCount = m_likesDeserializer.ParseLikesCount(responseData);

                return likesCount;
            }
            catch
            {
                throw new VkException("Failed to like post");
            }
        }

        public string GetAuthUrl(int _applicationId, Permissions _permissions)
        {
            return RequestBuilder.BuildAuthString(_applicationId, _permissions, m_currentVkVersion);
        }
    }
}
