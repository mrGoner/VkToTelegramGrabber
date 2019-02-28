using System;
using System.Collections.Generic;
using System.Linq;
using VkTools.ObjectModel.Wall;
using VkTools.ObjectModel.Attachments.Photo;
using VkTools.ObjectModel.Attachments;
using VkGrabber.Model;
using Post = VkGrabber.Model.Post;

namespace VkGrabber.Converters
{
    public class NewsFeedToPostsConverter
    {
        public Posts Convert(NewsFeed _newsFeed)
        {
            if (_newsFeed == null)
                throw new ArgumentNullException(nameof(_newsFeed));

            var posts = new Posts();

            foreach (var element in _newsFeed)
            {
                if (element is VkTools.ObjectModel.Wall.Post vkPost)
                {
                    Post post;
                    if (vkPost.CopyHistory != null && vkPost.CopyHistory.Any())
                    {
                        var copyHistory = vkPost.CopyHistory.First();
                        post = new Post(copyHistory.Date, "null", copyHistory.Text,
                            ParseAttachments(copyHistory.Attachments).ToArray());
                    }
                    else
                        post = new Post(vkPost.Date, "null", vkPost.Text,
                            ParseAttachments(vkPost.Attachments).ToArray());

                    posts.Add(post);
                }
            }

            return posts;
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
                        case VkTools.ObjectModel.Attachments.Photo.PhotoAttachment photo:
                            var smallPhotoUrl = photo.Sizes.FirstOrDefault(_size => _size.Type == PhotoSizeType.y).Url;
                            var mediumPhotoUrl = photo.Sizes.FirstOrDefault(_size => _size.Type == PhotoSizeType.z).Url;
                            var largePhotoUrl = photo.Sizes.FirstOrDefault(_size => _size.Type == PhotoSizeType.w).Url;
                            items.Add(new ImageItem(photo.Text, smallPhotoUrl, mediumPhotoUrl, largePhotoUrl));
                            break;
                        case VkTools.ObjectModel.Attachments.Audio.AudioAttachment audio:
                            items.Add(new AudioItem(audio.Title, audio.Artist, audio.Url));
                            break;
                        case VkTools.ObjectModel.Attachments.Doc.DocumentAttachment document:
                            items.Add(new DocumentItem(document.Title, document.Url));
                            break;
                        case VkTools.ObjectModel.Attachments.Link.LinkAttachment link:
                            items.Add(new LinkItem(link.Url));
                            break;
                        case VkTools.ObjectModel.Attachments.Video.VideoAttachment video:
                            items.Add(new VideoItem(video.Title, video.PlayerUrl));
                            break;
                    }
                }
            }

            return items;
        }
    }
}