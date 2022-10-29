using System;
using VkApi.ObjectModel;
using VkApi.ObjectModel.Attachments.Video;
using VkApi.ObjectModel.Wall;
using VkApi.Requests;
using VkApi.Serializers;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace VkApi
{
    public class Vk
    {
        private readonly NewsFeedDeserializer m_newsFeedDeserializer;
        private readonly GroupsDeserializer m_groupsDeserializer;
        private readonly LikesDeserializer m_likesDeserializer;
        private readonly VideoInfoDeserializer m_videoInfoDeserializer;
        private const string CurrentVkVersion = "5.131";
        private const string BaseUrl = "https://api.vk.com/method";

        public Vk()
        {
            m_newsFeedDeserializer = new NewsFeedDeserializer();
            m_groupsDeserializer = new GroupsDeserializer();
            m_likesDeserializer = new LikesDeserializer();
            m_videoInfoDeserializer = new VideoInfoDeserializer();
        }

        public async Task<NewsFeed> GetNewsFeedAsync(string _userToken, DateTime _start, DateTime _end, string _sourceIds, CancellationToken _cancellationToken)
        {
            try
            {
                var request = RequestBuilder.BuildNewsFeedRequest(_userToken, CurrentVkVersion,
                    _start, _end, _sourceIds);

                using var requestExecutor = new RestClient(BaseUrl);

                var responseData = await requestExecutor.ExecuteGetAsync(new RestRequest(request), _cancellationToken);

                var newsFeed = m_newsFeedDeserializer.Deserialize(responseData.Content);

                var videosToEnrich = new List<VideoAttachment>(10);

                foreach (var newsFeedElement in newsFeed)
                {
                    if (newsFeedElement is Post post)
                        videosToEnrich.AddRange(ExtractVideoAttachmentsFromPost(post));
                }

                await EnrichVideoItems(_userToken, videosToEnrich, _cancellationToken);

                return newsFeed;
            }
            catch (DeserializerException)
            {
                throw;
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
            catch (DeserializerException)
            {
                throw;
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
            catch (DeserializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VkException($"Failed to like post for owner {_itemOwner} itemId {_itemId}", ex);
            }
        }

        public string GetAuthUrl(int _applicationId, Permissions _permissions)
        {
            return RequestBuilder.BuildAuthString(_applicationId, _permissions, CurrentVkVersion);
        }

        private async Task EnrichVideoItems(string _userToken, IEnumerable<VideoAttachment> videoAttachments, CancellationToken _cancellationToken)
        {
            using var requestExecutor = new RestClient(BaseUrl);

            foreach (var videoAttachment in videoAttachments)
            {
                //todo getvideos by ownerId
                var getVideoRequest = RequestBuilder.BuildGetVideoRequest(_userToken, CurrentVkVersion, videoAttachment.OwnerId, videoAttachment.Id);

                var responseData = await requestExecutor.ExecuteGetAsync(new RestRequest(getVideoRequest), _cancellationToken);

                var videoInfo = m_videoInfoDeserializer.Deserialize(responseData.Content);

                videoAttachment.PlayerUrl = videoInfo?.PlayerUrl;
            }
        }

        private IReadOnlyCollection<VideoAttachment> ExtractVideoAttachmentsFromPost(Post post)
        {
            var videosToEnrich = new List<VideoAttachment>();

            var videoAttachments = post.Attachments.OfType<VideoAttachment>().Where(video => string.IsNullOrWhiteSpace(video.PlayerUrl) && video.IsContentRestricted == false);

            videosToEnrich.AddRange(videoAttachments);

            foreach (var copyHistory in post.CopyHistory)
                videosToEnrich.AddRange(ExtractVideoAttachmentsFromPost(copyHistory));

            return videosToEnrich;
        }
    }
}
