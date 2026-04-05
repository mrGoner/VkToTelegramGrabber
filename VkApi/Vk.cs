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

namespace VkApi;

public class Vk
{
    private readonly NewsFeedItemsDeserializer _newsFeedItemsDeserializer = new();
    private readonly GroupsDeserializer _groupsDeserializer = new();
    private readonly LikesDeserializer _likesDeserializer = new();
    private readonly VideoInfoDeserializer _videoInfoDeserializer = new();
    private const string CurrentVkVersion = "5.199";
    private const string BaseUrl = "https://api.vk.ru/method";

    public async Task<NewsFeed> GetNewsFeedAsync(string userToken, DateTime start, DateTime end, string sourceIds,
        CancellationToken cancellationToken)
    {
        try
        {
            using var requestExecutor = new RestClient(BaseUrl);

            var fetchedItems = new List<INewsFeedElement>(10);
            string? nextToken = null;

            do
            {
                var request = RequestBuilder.BuildNewsFeedRequest(userToken, CurrentVkVersion,
                    start, end, sourceIds, nextToken == null ? 50 : 70, nextToken);

                var response = await requestExecutor.ExecuteGetAsync(new RestRequest(request), cancellationToken);

                ThrowIfNotSuccessfulResponse(response);

                var itemsWithToken = _newsFeedItemsDeserializer.Deserialize(response.Content!);

                fetchedItems.AddRange(itemsWithToken.Items);
                nextToken = itemsWithToken.NextToken;

                if (!string.IsNullOrWhiteSpace(nextToken))
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // for avoiding request limiter

            } while (!string.IsNullOrWhiteSpace(nextToken));

            var videosToEnrich = new List<VideoAttachment>(10);

            foreach (var item in fetchedItems)
            {
                if (item is Post post)
                    videosToEnrich.AddRange(ExtractVideoAttachmentsFromPost(post));
            }

            await EnrichVideoItems(userToken, videosToEnrich, cancellationToken);

            return new NewsFeed(fetchedItems);
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

    public async Task<Groups> GetGroupsAsync(string userToken, int count, CancellationToken cancellationToken)
    {
        try
        {
            var request = RequestBuilder.BuildGroupRequest(userToken, CurrentVkVersion, count);

            using var requestExecutor = new RestClient(BaseUrl);

            var response = await requestExecutor.ExecuteGetAsync(new RestRequest(request), cancellationToken);

            ThrowIfNotSuccessfulResponse(response);

            var groups = _groupsDeserializer.Deserialize(response.Content!);

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

    public string GetAuthUrl(int applicationId, Permissions permissions) =>
        RequestBuilder.BuildAuthString(applicationId, permissions, CurrentVkVersion);

    private async Task EnrichVideoItems(string userToken, IEnumerable<VideoAttachment> videoAttachments,
        CancellationToken cancellationToken)
    {
        using var requestExecutor = new RestClient(BaseUrl);

        foreach (var videoAttachment in videoAttachments)
        {
            //todo getvideos by ownerId
            var getVideoRequest = RequestBuilder.BuildGetVideoRequest(userToken, CurrentVkVersion,
                videoAttachment.OwnerId, videoAttachment.Id);

            var response = await requestExecutor.ExecuteGetAsync(new RestRequest(getVideoRequest), cancellationToken);

            ThrowIfNotSuccessfulResponse(response);
            
            var videoInfo = _videoInfoDeserializer.Deserialize(response.Content!);

            videoAttachment.PlayerUrl = videoInfo?.PlayerUrl;

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); //to avoid request limiter 
        }
    }

    private static IReadOnlyCollection<VideoAttachment> ExtractVideoAttachmentsFromPost(Post post)
    {
        var videosToEnrich = new List<VideoAttachment>();

        var videoAttachments = post.Attachments.OfType<VideoAttachment>().Where(video =>
            string.IsNullOrWhiteSpace(video.PlayerUrl) && !video.IsContentRestricted);

        videosToEnrich.AddRange(videoAttachments);

        foreach (var copyHistory in post.CopyHistory)
            videosToEnrich.AddRange(ExtractVideoAttachmentsFromPost(copyHistory));

        return videosToEnrich;
    }
    
    private static void ThrowIfNotSuccessfulResponse(RestResponse response)
    {
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Response not indicate success. Status: {response.StatusCode} Response content: {response.Content}");
    }
}