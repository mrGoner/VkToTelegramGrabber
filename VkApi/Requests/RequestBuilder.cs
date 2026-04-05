using System;
using VkApi.Converters;
using VkApi.ObjectModel;
using VkApi.Extensions;
using System.Linq;

namespace VkApi.Requests;

internal static class RequestBuilder
{
    public static string BuildGroupRequest(string token, string apiVersion, int count, int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token can not be null or empty!");
        if (string.IsNullOrWhiteSpace(apiVersion))
            throw new ArgumentException("Api version can not be null or empty!");

        var groupRequest = $"groups.get?extended=1&offset={offset}&access_token={token}&v={apiVersion}&count={count}";

        return groupRequest;
    }

    public static string BuildNewsFeedRequest(string token, string apiVersion, DateTime startTime, DateTime endTime,
        string sourceIds, int count = 50, string? nextToken = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token can not be null or empty!");

        if (string.IsNullOrWhiteSpace(apiVersion))
            throw new ArgumentException("Api version can not be null or empty!");

        if (string.IsNullOrWhiteSpace(sourceIds))
            throw new ArgumentException("SourceIds can not be null or empty!");

        var epochStartTime = EpochTimeConverter.ConvertFromDateTime(startTime);
        var epochEndTime = EpochTimeConverter.ConvertFromDateTime(endTime);

        var newsfeedRequest =
            $"newsfeed.get?filters=post&access_token={token}&v={apiVersion}&start_time={epochStartTime}&end_time={epochEndTime}&source_ids={sourceIds}&count={count}";

        if (nextToken != null)
            newsfeedRequest += $"&start_from={nextToken}";

        return newsfeedRequest;
    }

    public static string BuildGetVideoRequest(string token, string apiVersion, int ownerId, int videoId)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token can not be null or empty!");

        if (string.IsNullOrWhiteSpace(apiVersion))
            throw new ArgumentException("Api version can not be null or empty!");

        var video = $"{ownerId}_{videoId}";
        var videoRequest = $"video.get?extended=0&videos={video}&access_token={token}&v={apiVersion}&count=1";

        return videoRequest;
    }


    public static string BuildAuthString(int applicationId, Permissions permissions, string apiVersion)
    {
        if (string.IsNullOrWhiteSpace(apiVersion))
            throw new ArgumentException("Api version can not be null or white space!");

        var scope = string.Join(',', permissions.GetFlags().Select(x => x.ToString().ToLowerInvariant()));

        var url =
            $"https://oauth.vk.ru/authorize?client_id={applicationId}&display=page&redirect_uri=https://oauth.vk.ru/blank.html&response_type=token&v={apiVersion}&scope={scope}";

        return url;
    }
}