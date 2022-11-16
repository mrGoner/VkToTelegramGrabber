using System;
using VkApi.Converters;
using VkApi.ObjectModel;
using VkApi.Extensions;
using System.Linq;

namespace VkApi.Requests
{
    internal static class RequestBuilder
    {
        public static string BuildGroupRequest(string _token, string _apiVersion, int count, int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            var groupRequest = $"groups.get?extended=1&offset={offset}&access_token={_token}&v={_apiVersion}&count={count}";

            return groupRequest;
        }

        public static string BuildNewsFeedRequest(string _token, string _apiVersion,
                                                  DateTime _startTime, DateTime _endTime,
                                                  string _sourceIds, int _count = 50, string nextToken = null)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");

            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            if (string.IsNullOrWhiteSpace(_sourceIds))
                throw new ArgumentException("SourceIds can not be null or empty!");

            var epochStartTime = EpochTimeConverter.ConvertFromDateTime(_startTime);
            var epochEndTime = EpochTimeConverter.ConvertFromDateTime(_endTime);

            var newsfeedRequest = $"newsfeed.get?filters=post&access_token={_token}&v={_apiVersion}&start_time={epochStartTime}&end_time={epochEndTime}&source_ids={_sourceIds}&count={_count}";

            if (nextToken != null)
                newsfeedRequest += $"start_from={nextToken}";

            return newsfeedRequest;
        }

        public static string BuildGetVideoRequest(string _token, string _apiVersion, int _ownerId, int _videoId)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");

            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            var video = $"{_ownerId}_{_videoId}";
            var videoRequest = $"video.get?extended=0&videos={video}&access_token={_token}&v={_apiVersion}&count=1";

            return videoRequest;
        }


        public static string BuildAuthString(int _applicationId, Permissions _permissions, string _apiVersion)
        {
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or white space!");

            var scope = string.Join(',', _permissions.GetFlags().Select(_x => _x.ToString().ToLowerInvariant()));

            var url = $"https://oauth.vk.com/authorize?client_id={_applicationId}&display=page&redirect_uri=https://oauth.vk.com/blank.html&response_type=token&v={_apiVersion}&scope={scope}";

            return url;
        }

        public static string BuildLikeRequest(LikeType _type, int _ownerId, uint _itemId, string _token, string _apiVersion)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty", nameof(_token));
                
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            var url = $"likes.add?type={_type.ConvertToSnakeCase()}&owner_id={_ownerId}&item_id={_itemId}&access_token={_token}&v={_apiVersion}";

            return url;
        }
    }
}