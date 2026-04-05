using System;
using System.Collections.Generic;
using System.Linq;
using VkApi.ObjectModel.Attachments;
using VkApi.ObjectModel.Attachments.Audio;
using VkApi.ObjectModel.Attachments.Doc;
using VkApi.ObjectModel.Attachments.Link;
using VkApi.ObjectModel.Attachments.Note;
using VkApi.ObjectModel.Attachments.Photo;
using VkApi.ObjectModel.Attachments.Video;
using VkApi.ObjectModel.Wall;
using VkGrabber.Model;
using Post = VkGrabber.Model.Post;

namespace VkGrabber.Converters;

public class NewsFeedToPostsConverter
{
    public IEnumerable<Post> Convert(NewsFeed newsFeed, Dictionary<int, string> groups)
    {
        if (newsFeed == null)
            throw new ArgumentNullException(nameof(newsFeed));

        if (groups == null)
            throw new ArgumentNullException(nameof(groups));

        foreach (var element in newsFeed)
        {
            if (element is not VkApi.ObjectModel.Wall.Post { MarkedAsAds: false } vkPost) 
                continue;
            
            Post post;

            var clearSourceId = Math.Abs(vkPost.SourceId);

            if (!groups.TryGetValue(clearSourceId, out var groupName))
                continue;

            if (vkPost.CopyHistory is { Length: > 0 })
            {
                var copyHistory = vkPost.CopyHistory.First();
                //merge maybe
                post = new Post(vkPost.Id, clearSourceId, copyHistory.Date, groupName, copyHistory.Text,
                    ParseAttachments(copyHistory.Attachments).ToArray());
            }
            else
            {
                post = new Post(vkPost.Id, clearSourceId, vkPost.Date, groupName, vkPost.Text,
                    ParseAttachments(vkPost.Attachments).ToArray());
            }

            yield return post;
        }
    }

    private static List<IPostItem> ParseAttachments(IAttachmentElement[] attachments)
    {
        var items = new List<IPostItem>();
        
        foreach (var vkAttachment in attachments)
        {
            switch (vkAttachment)
            {
                case PhotoAttachment photo:
                    var smallPhotoUrl = photo.Sizes.FirstOrDefault(size => size.Type == PhotoSizeType.X).Url;
                    var mediumPhotoUrl = photo.Sizes.FirstOrDefault(size => size.Type == PhotoSizeType.Y).Url;
                    var largePhotoUrl = photo.Sizes.FirstOrDefault(size => size.Type == PhotoSizeType.Z).Url;

                    if (!string.IsNullOrWhiteSpace(smallPhotoUrl) || !string.IsNullOrWhiteSpace(mediumPhotoUrl) || !string.IsNullOrWhiteSpace(largePhotoUrl))
                        items.Add(new ImageItem(photo.Text, smallPhotoUrl, mediumPhotoUrl, largePhotoUrl));
                    break;
                case AudioAttachment { Url.Length: > 0 } audio:
                    items.Add(new AudioItem(audio.Title, audio.Artist, audio.Url));
                    break;
                case DocumentAttachment { Url.Length: > 0 } document:
                    items.Add(new DocumentItem(document.Title, document.Url));
                    break;
                case LinkAttachment { Url.Length: > 0 } link:
                    items.Add(new LinkItem(link.Url));
                    break;
                case VideoAttachment video:
                    if (video.IsContentRestricted || string.IsNullOrWhiteSpace(video.PlayerUrl))
                        continue;
                    items.Add(new VideoItem(video.Title, video.PlayerUrl));
                    break;
                case NoteAttachment { Text: not null } note:
                    items.Add(new NoteItem(note.Title, note.Text));
                    break;
            }
        }

        return items;
    }
}