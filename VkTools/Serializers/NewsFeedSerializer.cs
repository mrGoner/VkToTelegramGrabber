using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using VkTools.ObjectModel;
using VkTools.ObjectModel.Attachments;
using VkTools.ObjectModel.Attachments.Photo;
using VkTools.ObjectModel.Wall;

namespace VkTools.Serializers
{
    public partial class NewsFeedSerializer
    {
        private readonly EpochTimeConverter m_timeConverter;

        public NewsFeedSerializer()
        {
            m_timeConverter = new EpochTimeConverter();
        }

        public NewsFeed Deserialize(string _data)
        {
            var jObject = JObject.Parse(_data);


            if (jObject[PResponse] is JObject response)
            {
                if (response[PItems] is JArray jItems)
                {
                    var newsFeed = new NewsFeed();

                    foreach (JObject jItem in jItems)
                    {
                        var itemRawType = jItem[PItemType].Value<string>();

                        if (itemRawType == "post")
                            newsFeed.Add(ParsePostItem(jItem));
                    }

                    return newsFeed;
                }
            }

            throw new ArgumentException();
        }

        private Post ParsePostItem(JObject _jPostItem)
        {
            var post = new Post();

            post.SourceId = _jPostItem[PItemId].Value<int>();
            post.Date = m_timeConverter.ConvertToDateTime(_jPostItem[PItemDate].Value<int>());
            post.PostId = _jPostItem[PItemId].Value<int>();
            post.PostType = _jPostItem[PItemPostType].Value<string>();
            post.Text = _jPostItem[PItemText].Value<string>();
            post.SignerId = _jPostItem[PItemSignerId]?.Value<int>() ?? null;
            post.MarkedAsAds = _jPostItem[PItemMarkedAsAds].Value<int>() != 0;

            if (_jPostItem[PAttachments] is JArray jAttachments)
                post.Attachments = ParseAttachments(jAttachments).ToArray();

            return post;
        }

        private Comments ParseComments(JObject _comments)
        {
            var comments = new Comments
            {

            };

            return comments;
        }

        private PostSource ParsePostSource(JObject _jPostSource)
        {
            var postSource = new PostSource();

            var postSourceRawType = _jPostSource[PPostSourceType].Value<string>();
            
            switch (postSourceRawType)
            {
                case "vk":
                    postSource.Type = PostSourceType.Vk;
                    break;
                case "widget":
                    postSource.Type = PostSourceType.Widget;
                    break;
                case "api":
                    postSource.Type = PostSourceType.Api;
                    break;
                case "rss":
                    postSource.Type = PostSourceType.Rss;
                     break;
                case "sms":
                    postSource.Type = PostSourceType.Sms;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var rawPlatformType = _jPostSource[PPostSourcePlatform]?.Value<string>();

            if(rawPlatformType != null)
            {
                switch (rawPlatformType)
                {
                    case "android":
                        postSource.Platfrom = PlatformType.Android;
                        break;
                    case "iphone":
                        postSource.Platfrom = PlatformType.Iphone;
                        break;
                    case "wphone":
                        postSource.Platfrom = PlatformType.WPhone;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (postSource.Type == PostSourceType.Widget || postSource.Type == PostSourceType.Vk)
            {
                postSource.Data = _jPostSource[PPostSourceData]?.Value<string>();
            }

            postSource.Url = _jPostSource[PPostSourceUrl]?.Value<string>();

            return postSource;
        }

        private Likes ParseLikes(JObject _jLikes)
        {
            var count = _jLikes[PLikesCount].Value<int>();
            bool userLikes = _jLikes[PLikesUserLikes].Value<int>() != 0;
            bool canLike = _jLikes[PLikesCanLike].Value<int>() != 0;
            bool canPublish = _jLikes[PLikesCanPublish].Value<int>() != 0;

            return new Likes(count, userLikes, canLike, canPublish);
        }

        private Reposts ParseReposts(JObject _reposts)
        {
            return new Reposts();
        }

        private Views ParseViews(JObject _views)
        {
            return new Views();
        }

        private List<AttachmentElement> ParseAttachments(JArray _jAttachments)
        {
            var attachments = new List<AttachmentElement>();

            foreach(JObject jAttachment in _jAttachments)
            {
                var attachmentRawType = jAttachment[PAttachmentsType].Value<string>();

                if (attachmentRawType == "photo")
                    attachments.Add(ParsePhotoAttachment(jAttachment));
            }

            return attachments;
        }

        private PhotoAttachment ParsePhotoAttachment(JObject _jPhoto)
        {
            var photoAttachment = new PhotoAttachment();

            if(_jPhoto[PAttachmentsPhoto] is JObject photoJObj)
            {
                var photoInfo = new PhotoInfo();

                photoInfo.Id = photoJObj[PPhotoId].Value<int>();
                photoInfo.AlbumId = photoJObj[PPhotoAlbumId].Value<int>();
                photoInfo.OwnerId = photoJObj[PPhotoOwnerId].Value<int>();
               //todo add missing fields
                var sizes = new List<PhotoSizeInfo>();

                if (photoJObj[PPhotoSizes] is JArray jSizes)
                {
                    foreach (var jSize in jSizes)
                    {
                        var size = new PhotoSizeInfo
                        {
                            Type = (PhotoSizeType)Enum.Parse(typeof(PhotoSizeType), jSize[PSizesType].Value<string>()),
                            Url = jSize[PSizesUrl].Value<string>(),
                            Width = jSize[PSizesWidth].Value<int>(),
                            Height = jSize[PSizesHeight].Value<int>()
                        };

                        sizes.Add(size);
                    }
                }

                photoInfo.Sizes = sizes.ToArray();
                photoAttachment.Info = photoInfo;
            }

            return photoAttachment;
        }
    }

    internal class EpochTimeConverter
    {
        private readonly DateTime m_startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public DateTime ConvertToDateTime(int _seconds)
        {
            return m_startTime.AddSeconds(_seconds);
        }
    }
}
