using System;
using VkTools;

namespace VkApi.Requests
{
    public static class RequestBuilder
    {
        private const string GroupTemplate = "groups.get?extended=1&offset={0}&access_token={1}&v={2}&count={3}";
        private const string NewsFeedTemplate = "newsfeed.get?filters=post&access_token={0}&v={1}&start_time={2}&end_time={3}&source_ids={4}&count={5}";

        public static string BuildGroupRequest(string _token, string _apiVersion, int count, int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or empty!");
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or empty!");

            var groupRequest = string.Format(GroupTemplate, offset, _token, _apiVersion, count);

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

            var newsfeedRequest = string.Format(NewsFeedTemplate, _token, 
                            _apiVersion, epochStartTime, epochEndTime, _sourceIds, _count);

            return newsfeedRequest;
        }
    }
}
