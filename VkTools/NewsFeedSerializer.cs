using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace VkTools
{
    public class NewsFeedSerializer : INewsFeedSerializer
    {
        public const string PResponse = "response";
        public const string PItems = "items";
        public const string PItemType = "type";
        public const string PItemDate = "date";
        public const string PItemId = "post_id";
        public const string PItemPostType = "post_type";
        public const string PItemText = "text";
        public const string PItemSignerId = "signer_id";
        public const string PItemMarkedAsAds = "marked_as_ads";
        public const string PAttachments = "attachments";
        public const string PAttachmentsType = "type";
        public const string PAttachmentsPhoto = "photo";
        public const string PPhotoId = "id";
        public const string PPhotoAlbumId = "album_id";
        public const string PPhotoOwnerId = "owner_id";
        public const string PPhotoUserId = "user_id";
        public const string PPhotoSizes = "sizes";
        public const string PSizesType = "type";
        public const string PSizesUrl = "url";
        public const string PSizesWidth = "width";
        public const string PSizesHeight = "height";
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

        public string Serialize(NewsFeed _object)
        {
            throw new NotImplementedException();
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

        private PostSource ParsePostSource(JObject _postSource)
        {
            return null;
        }

        private Comments ParseComments(JObject _comments)
        {
            var comments = new Comments
            {

            };

            return comments;
        }

        private Likes ParseLikes(JObject _likes)
        {
            return new Likes();
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

    public interface INewsFeedSerializer
    {
        string Serialize(NewsFeed _object);
        NewsFeed Deserialize(string _data);
    }
}
