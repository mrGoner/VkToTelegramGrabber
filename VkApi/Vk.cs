using System;
using VkApi.ObjectModel;
using VkApi.ObjectModel.Wall;
using VkApi.Requests;
using VkApi.Serializers;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;

namespace VkApi
{
    public class Vk
    {
        private readonly NewsFeedDeserializer m_newsFeedDeserializer;
        private readonly GroupsDeserializer m_groupsDeserializer;
        private readonly LikesDeserializer m_likesDeserializer;
        private const string CurrentVkVersion = "5.131";
        private const string BaseUrl = "https://api.vk.com/method";

        public Vk()
        {
            m_newsFeedDeserializer = new NewsFeedDeserializer();
            m_groupsDeserializer = new GroupsDeserializer();
            m_likesDeserializer = new LikesDeserializer();
        }

        public async Task<NewsFeed> GetNewsFeedAsync(string _userToken, DateTime _start, DateTime _end, string _sourceIds, CancellationToken cancellationToken)
        {
            try
            {
                var request = RequestBuilder.BuildNewsFeedRequest(_userToken, CurrentVkVersion,
                                 _start, _end, _sourceIds);

                using var requestExecutor = new RestClient(BaseUrl);

                var responseData = await requestExecutor.ExecuteGetAsync(new RestRequest(request), cancellationToken);

                var newsFeed = m_newsFeedDeserializer.Deserialize(responseData.Content);

                // var getVideoFunc = new Func<VideoInfo, string>((_arg) =>
                // {
                //     try
                //     {
                //         var getVideoRequest = RequestBuilder.BuildGetVideoRequest(_userToken, CurrentVkVersion, _arg.OwnerId, _arg.VideoId);

                //         var videoResponseData = m_requestExecutor.Execute(getVideoRequest, cancellationToken);

                //         return videoResponseData;
                //     }
                //     catch (Exception ex)
                //     {
                //         throw new VkException("Failed to get video", ex);
                //     }
                // });

                return newsFeed;
            }
            catch (Exception ex)
            {
                throw new VkException("Failed to get newsfeed", ex);
            }
        }

        public async Task<Groups> GetGroupsAsync(string _userToken, int _count, CancellationToken cancellationToken)
        {
            try
            {
                var request = RequestBuilder.BuildGroupRequest(_userToken, CurrentVkVersion, _count);

                using var requestExecutor = new RestClient(BaseUrl);

                var responseData = await requestExecutor.ExecuteGetAsync(new RestRequest(request), cancellationToken);

                var groups = m_groupsDeserializer.Deserialize(responseData.Content);

                return groups;
            }
            catch (Exception ex)
            {
                throw new VkException("Failed to get groups", ex);
            }
        }

        public async Task<int> LikePostAsync(int _itemOwner, uint _itemId, string _userToken, CancellationToken cancellationToken)
        {
            try
            {
                var request = RequestBuilder.BuildLikeRequest(LikeType.Post, _itemOwner, _itemId, _userToken, CurrentVkVersion);

                using var requestExecutor = new RestClient(BaseUrl);

                var responseData = await requestExecutor.ExecuteGetAsync(new RestRequest(request), cancellationToken);

                var likesCount = m_likesDeserializer.ParseLikesCount(responseData.Content);

                return likesCount;
            }
            catch(Exception ex)
            {
                throw new VkException($"Failed to like post for owner {_itemOwner} itemId {_itemId}", ex);
            }
        }

        public string GetAuthUrl(int _applicationId, Permissions _permissions)
        {
            return RequestBuilder.BuildAuthString(_applicationId, _permissions, CurrentVkVersion);
        }
    }
}
