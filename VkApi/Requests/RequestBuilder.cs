﻿using System;
using VkTools;

namespace VkApi.Requests
{
    internal static class RequestBuilder
    {
        private const string m_groupTemplate = "groups.get?extended=1&offset={0}&access_token={1}&v={2}&count={3}";
        private const string m_newsFeedTemplate = "newsfeed.get?filters=post&access_token={0}&v={1}&start_time={2}&end_time={3}&source_ids={4}&count={5}";
        private const string m_urlTemplate = "https://oauth.vk.com/authorize?client_id={0}&display=page&redirect_uri=https://oauth.vk.com/blank.html&response_type=token&v={1}&scope={2}";
        private const string m_getVideoTemplate = "video.get?extended=0&videos={0}&access_token={1}&v={2}&count={3}";
        private const string m_addLikeTemplate = "likes.add?type={0}&owner_id={1}&item_id={2}&access_token={3}&v={4}";

        public static string BuildGroupRequest(string _token, string _apiVersion, int count, int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            var groupRequest = string.Format(m_groupTemplate, offset, _token, _apiVersion, count);

            return groupRequest;
        }

        public static string BuildNewsFeedRequest(string _token, string _apiVersion,
                                                  DateTime _startTime, DateTime _endTime,
                                                  string _sourceIds, int _count = 50)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");

            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            if (string.IsNullOrWhiteSpace(_sourceIds))
                throw new ArgumentException("SourceIds can not be null or empty!");

            var epochStartTime = EpochTimeConverter.ConvertFromDateTime(_startTime);
            var epochEndTime = EpochTimeConverter.ConvertFromDateTime(_endTime);

            var newsfeedRequest = string.Format(m_newsFeedTemplate, _token,
                            _apiVersion, epochStartTime, epochEndTime, _sourceIds, _count);

            return newsfeedRequest;
        }

        public static string BuildGetVideoRequest(string _token, string _apiVersion, int _ownerId, int _videoId)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");

            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            var video = $"{_ownerId}_{_videoId}";
            var videoRequest = string.Format(m_getVideoTemplate, video, _token, _apiVersion, 1);

            return videoRequest;
        }


        public static string BuildAuthString(int _applicationId, Permissions _permissions, string _apiVersion)
        {
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or white space!");

            var scope = "";

            if (_permissions.HasFlag(Permissions.Friends))
                scope += "friends,";
            if (_permissions.HasFlag(Permissions.Photos))
                scope += "photos,";
            if (_permissions.HasFlag(Permissions.Audio))
                scope += "audio,";
            if (_permissions.HasFlag(Permissions.Video))
                scope += "video,";
            if (_permissions.HasFlag(Permissions.Messages))
                scope += "messages,";
            if (_permissions.HasFlag(Permissions.Offline))
                scope += "offline,";
            if (_permissions.HasFlag(Permissions.Groups))
                scope += "groups,";
            if (_permissions.HasFlag(Permissions.Docs))
                scope += "docs,";
            if (_permissions.HasFlag(Permissions.Wall))
                scope += "wall";

            scope = scope.TrimEnd(',');

            var url = string.Format(m_urlTemplate, _applicationId, _apiVersion, scope);

            return url;
        }

        public static string BuildLikeRequest(LikeType _type, int _ownerId, uint _itemId, string _token, string _apiVersion)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty", nameof(_token));

            var url = string.Format(m_addLikeTemplate, _type.ToString().ToLowerInvariant(), _ownerId, _itemId, _token, _apiVersion);

            return url;
        }
    }
}