using System;
using System.Collections.Generic;
using System.Linq;
using VkApi.ObjectModel.Attachments;
using VkApi.ObjectModel.Attachments.Audio;
using VkApi.ObjectModel.Attachments.Doc;
using VkApi.ObjectModel.Attachments.Link;
using VkApi.ObjectModel.Attachments.Photo;
using VkApi.ObjectModel.Attachments.Video;
using VkApi.ObjectModel.Wall;
using VkGrabber.Model;
using Post = VkGrabber.Model.Post;

namespace VkGrabber.Converters
{
    public class NewsFeedToPostsConverter
    {
        public IEnumerable<Post> Convert(NewsFeed _newsFeed, Dictionary<int, string> _groups)
        {
            if (_newsFeed == null)
                throw new ArgumentNullException(nameof(_newsFeed));

            if (_groups == null)
                throw new ArgumentNullException(nameof(_groups));

            foreach (var element in _newsFeed)
            {
                if (element is VkApi.ObjectModel.Wall.Post vkPost && !vkPost.MarkedAsAds)
                {
                    Post post;

                    var clearSourceId = Math.Abs(vkPost.SourceId);

                    _groups.TryGetValue(clearSourceId, out var groupName);

                    if (vkPost.CopyHistory != null && vkPost.CopyHistory.Any())
                    {
                        var copyHistory = vkPost.CopyHistory.First();
                        //merge maybe
                        post = new Post(vkPost.Id, clearSourceId, copyHistory.Date, groupName, copyHistory.Text,
                            ParseAttachments(copyHistory.Attachments).ToArray());
                    }
                    else
                        post = new Post(vkPost.Id, clearSourceId, vkPost.Date, groupName, vkPost.Text,
                            ParseAttachments(vkPost.Attachments).ToArray());

                    yield return post;
                }
            }
        }

        private List<IPostItem> ParseAttachments(IAttachmentElement[] _attachments)
        {
            var items = new List<IPostItem>();

            if (_attachments != null)
            {
                foreach (var vkAttachment in _attachments)
                {
                    switch (vkAttachment)
                    {
                        case PhotoAttachment photo:
                            var smallPhotoUrl = photo.Sizes.FirstOrDefault(_size => _size.Type == PhotoSizeType.x).Url;
                            var mediumPhotoUrl = photo.Sizes.FirstOrDefault(_size => _size.Type == PhotoSizeType.y).Url;
                            var largePhotoUrl = photo.Sizes.FirstOrDefault(_size => _size.Type == PhotoSizeType.z).Url;
                            items.Add(new ImageItem(photo.Text, smallPhotoUrl, mediumPhotoUrl, largePhotoUrl));
                            break;
                        case AudioAttachment audio:
                            items.Add(new AudioItem(audio.Title, audio.Artist, audio.Url));
                            break;
                        case DocumentAttachment document:
                            items.Add(new DocumentItem(document.Title, document.Url));
                            break;
                        case LinkAttachment link:
                            items.Add(new LinkItem(link.Url));
                            break;
                        case VideoAttachment video:
                            if (video.IsContentRestricted || string.IsNullOrEmpty(video.PlayerUrl))
                                continue;
                            items.Add(new VideoItem(video.Title, video.PlayerUrl));
                            break;
                    }
                }
            }

            return items;
        }
    }
}