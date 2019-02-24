using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using VkTools.ObjectModel;
using VkTools.ObjectModel.Attachments;
using VkTools.ObjectModel.Attachments.Audio;
using VkTools.ObjectModel.Attachments.Doc;
using VkTools.ObjectModel.Attachments.Link;
using VkTools.ObjectModel.Attachments.Photo;
using VkTools.ObjectModel.Attachments.Video;
using VkTools.ObjectModel.Wall;

namespace VkTools.Serializers
{
    public class NewsFeedDeserializer
    {
        #region consts
        public const string PResponse = "response";
        public const string PItems = "items";
        public const string PItemType = "type";
        public const string PItemDate = "date";
        public const string PItemId = "post_id";
        public const string PItemText = "text";
        public const string PItemSignerId = "signer_id";
        public const string PItemMarkedAsAds = "marked_as_ads";
        public const string PAttachments = "attachments";
        public const string PAttachmentsType = "type";
        public const string PAttachmentsPhoto = "photo";
        public const string PAttachmentsDocument = "doc";
        public const string PAttachmentsAudio = "audio";
        public const string PAttachmentsVideo = "video";
        public const string PAttachmentsLink = "link";
        public const string PPhotoAlbumId = "album_id";
        public const string PAttachmentOwnerId = "owner_id";
        public const string PId = "id";
        public const string PUrl = "url";
        public const string PDate = "date";
        public const string PTitle = "title";
        public const string PPhotoUserId = "user_id";
        public const string PPhotoText = "text";
        public const string PPhotoSizes = "sizes";
        public const string PSizesType = "type";
        public const string PSizesWidth = "width";
        public const string PSizesHeight = "height";
        public const string PComments = "comments";
        public const string PCommentsCount = "count";
        public const string PCommentsCanPost = "can_post";
        public const string PCommentsGroupsCanPost = "groups_can_post";
        public const string PLikes = "likes";
        public const string PLikesCount = "count";
        public const string PLikesUserLikes = "user_likes";
        public const string PLikesCanLike = "can_like";
        public const string PLikesCanPublish = "can_publish";
        public const string PReposts = "reposts";
        public const string PRepostsCount = "count";
        public const string PRepostsUserReposted = "user_reposted";
        public const string PViews = "views";
        public const string PViewsCount = "count";
        public const string PIsFavorite = "is_favorite";
        public const string PPostSource = "post_source";
        public const string PPostSourceType = "type";
        public const string PPostSourcePlatform = "platform";
        public const string PPostSourceData = "data";
        public const string PVideoDescription = "description";
        public const string PVideoDuration = "duration";
        public const string PVideoViews = "views";
        public const string PVideoComments = "comments";
        public const string PVideoPlayer = "player";
        public const string PVideoIsFavorite = "is_favorite";
        public const string PAudioArtist = "artist";
        public const string PLinkDescription = "description";
        public const string PAttachmentAccessKey = "access_key";
        public const string PCopyHistrory = "copy_history";
        public const string PHistoryOwnerId = "owner_id";
        public const string PHistoryFromId = "from_id";

        #endregion

        public NewsFeed Deserialize(string _data)
        {
            var jObject = JObject.Parse(_data);

            if (jObject[PResponse][PItems] is JArray jItems)
            {
                try
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
                catch(Exception ex)
                {
                    throw new DeserializerException("Failed to deserialize newsfeed", ex);
                }
            }

            throw new DeserializerException("Failed recognize jObject as vk response", _data);
        }

        private Post ParsePostItem(JObject _jPostItem)
        {
            var post = new Post();

            post.SourceId = _jPostItem[PItemId].Value<int>();
            post.Date = EpochTimeConverter.ConvertToDateTime(_jPostItem[PItemDate].Value<int>());
            post.PostId = _jPostItem[PItemId].Value<int>();
            post.Text = _jPostItem[PItemText].Value<string>();
            post.SignerId = _jPostItem[PItemSignerId]?.Value<int>() ?? null;
            post.MarkedAsAds = _jPostItem[PItemMarkedAsAds].Value<int>() != 0;
            post.PostSource = ParsePostSource((JObject)_jPostItem[PPostSource]);

            var attachmentsRaw = _jPostItem[PAttachments];
            if (attachmentsRaw != null)
                post.Attachments = ParseAttachments(attachmentsRaw).ToArray();

            post.Comments = ParseComments((JObject)_jPostItem[PComments]);
            post.Likes = ParseLikes((JObject)_jPostItem[PLikes]);
            post.Reposts = ParseReposts((JObject)_jPostItem[PReposts]);
            post.Views = ParseViews((JObject)_jPostItem[PViews]);

            var rawHistoryElem = _jPostItem[PCopyHistrory];

            if (rawHistoryElem != null)
                post.CopyHistory = ParseHistory(rawHistoryElem).ToArray();

            return post;
        }

        private List<HistoryPost> ParseHistory(JToken _jHistory)
        {
            if (_jHistory is JArray jCopyHistory)
            {
                var historyCollection = new List<HistoryPost>();

                foreach (JObject jHistoryElement in jCopyHistory)
                {
                    var historyPost = new HistoryPost();

                    historyPost.Id = jHistoryElement[PId].Value<int>();
                    historyPost.OwnerId = jHistoryElement[PHistoryOwnerId].Value<int>();
                    historyPost.FromId = jHistoryElement[PHistoryFromId].Value<int>();
                    historyPost.Date = EpochTimeConverter.ConvertToDateTime(jHistoryElement[PDate].Value<int>());
                    historyPost.Text = jHistoryElement[PItemText].Value<string>();
                    historyPost.PostSource = ParsePostSource((JObject) jHistoryElement[PPostSource]);

                    var attachmentsRaw = jHistoryElement[PAttachments];

                    if (attachmentsRaw != null)
                        historyPost.Attachments = ParseAttachments(attachmentsRaw).ToArray();

                    historyCollection.Add(historyPost);

                }

                return historyCollection;
            }
            throw new DeserializerException("History element not recognized as array", _jHistory?.ToString());
        }

        private Comments ParseComments(JObject _jComments)
        {
            int count = _jComments[PCommentsCount].Value<int>();
            bool canComment = _jComments[PCommentsCanPost].Value<int>() != 0;
            bool canGroupsComment = _jComments[PCommentsGroupsCanPost].Value<int>() != 0;

            return new Comments(count, canComment, canGroupsComment);
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
                        throw new ArgumentOutOfRangeException($"{rawPlatformType} out of range!");
                }

                if (postSource.Type == PostSourceType.Widget || postSource.Type == PostSourceType.Vk)
                {
                    postSource.Data = _jPostSource[PPostSourceData]?.Value<string>();
                }
            }

            postSource.Url = _jPostSource[PId]?.Value<string>();

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

        private Reposts ParseReposts(JObject _jReposts)
        {
            int count = _jReposts[PRepostsCount].Value<int>();
            bool userReposted = _jReposts[PRepostsUserReposted].Value<int>() != 0;

            return new Reposts(count, userReposted);
        }

        private Views ParseViews(JObject _jViews)
        {
            var count = _jViews[PViewsCount].Value<int>();

            return new Views(count);
        }

        private List<IAttachmentElement> ParseAttachments(JToken _jAttachments)
        {
            if (_jAttachments is JArray jAttachments)
            {
                var attachments = new List<IAttachmentElement>();

                foreach (JObject jAttachment in jAttachments)
                {
                    var attachmentRawType = jAttachment[PAttachmentsType].Value<string>();

                    switch (attachmentRawType)
                    {
                        case "photo":
                            attachments.Add(ParsePhotoAttachment(jAttachment));
                            break;
                        case "video":
                            attachments.Add(ParseVideoAttachment(jAttachment));
                            break;
                        case "doc":
                            attachments.Add(ParseDocAttachment(jAttachment));
                            break;
                        case "link":
                            attachments.Add(ParseLinkAttachment(jAttachment));
                            break;
                        case "audio":
                            attachments.Add(ParseAudioAttachment(jAttachment));
                            break;
                        default:
                            attachments.Add(new UnsupportedAttachment(attachmentRawType));
                            break;

                    }
                }

                return attachments;
            }
            throw new DeserializerException("Failed to recognize attachment token as jArray", _jAttachments?.ToString());
        }

        private PhotoAttachment ParsePhotoAttachment(JObject _jPhoto)
        {
            if(_jPhoto[PAttachmentsPhoto] is JObject photoJObj)
            {
                var photoAttachment = new PhotoAttachment();

                photoAttachment.Id = photoJObj[PId].Value<int>();
                photoAttachment.AlbumId = photoJObj[PPhotoAlbumId].Value<int>();
                photoAttachment.OwnerId = photoJObj[PAttachmentOwnerId].Value<int>();
                photoAttachment.UserId = photoJObj[PPhotoUserId]?.Value<int>();
                photoAttachment.Text = photoJObj[PPhotoText]?.Value<string>();
                photoAttachment.Date = EpochTimeConverter.ConvertToDateTime(photoJObj[PDate].Value<int>());
                photoAttachment.AccessKey = photoJObj[PAttachmentAccessKey]?.Value<string>();

                var sizes = new List<PhotoSizeInfo>();

                if (photoJObj[PPhotoSizes] is JArray jSizes)
                {
                    foreach (var jSize in jSizes)
                    {
                        var type = (PhotoSizeType)Enum.Parse(typeof(PhotoSizeType), jSize[PSizesType].Value<string>());
                        var url = jSize[PUrl].Value<string>();
                        var width = jSize[PSizesWidth].Value<int>();
                        var height = jSize[PSizesHeight].Value<int>();

                        var sizeInfo = new PhotoSizeInfo(type, url, width, height);

                        sizes.Add(sizeInfo);
                    }
                }

                photoAttachment.Sizes = sizes.ToArray();

                return photoAttachment;
            }

            throw new DeserializerException("Failed recognize jObject as photo attachment", _jPhoto?.ToString());
        }

        private DocumentAttachment ParseDocAttachment(JObject _jDoc)
        {
            if (_jDoc[PAttachmentsDocument] is JObject jDoc)
            {
                var docAttachment = new DocumentAttachment();

                docAttachment.Id = jDoc[PId].Value<int>();
                docAttachment.OwnerId = jDoc[PAttachmentOwnerId].Value<int>();
                docAttachment.Title = jDoc[PTitle].Value<string>();
                docAttachment.Date = EpochTimeConverter.ConvertToDateTime(jDoc[PDate].Value<int>());
                docAttachment.Url = jDoc[PUrl].Value<string>();
                docAttachment.AccessKey = jDoc[PAttachmentAccessKey]?.Value<string>();

                return docAttachment;
            }

            throw new DeserializerException("Failed recognize jObject as document attachment", _jDoc?.ToString());
        }

        private VideoAttachment ParseVideoAttachment(JObject _jVideo)
        {
            if (_jVideo[PAttachmentsVideo] is JObject jVideo)
            {
                var videoAttachment = new VideoAttachment();

                videoAttachment.Id = jVideo[PId].Value<int>();
                videoAttachment.OwnerId = jVideo[PAttachmentOwnerId].Value<int>();
                videoAttachment.Title = jVideo[PTitle].Value<string>();
                videoAttachment.Description = jVideo[PVideoDescription].Value<string>();
                videoAttachment.Duration = jVideo[PVideoDuration].Value<int>();
                videoAttachment.Date = EpochTimeConverter.ConvertToDateTime(jVideo[PDate].Value<int>());
                videoAttachment.Views = jVideo[PVideoViews].Value<int>();
                videoAttachment.CommentsCount = jVideo[PVideoComments].Value<int>();
                videoAttachment.PlayerUrl = jVideo[PVideoPlayer].Value<string>();
                videoAttachment.IsFavorite = jVideo[PVideoIsFavorite].Value<bool>();
                videoAttachment.AccessKey = jVideo[PAttachmentAccessKey]?.Value<string>();

                return videoAttachment;
            }

            throw new DeserializerException("Failed recognize jObject as video attachment", _jVideo?.ToString());
        }

        private AudioAttachment ParseAudioAttachment(JObject _jAudio)
        {
            if (_jAudio[PAttachmentsAudio] is JObject jAudio)
            {
                var audioAttachments = new AudioAttachment();

                audioAttachments.Id = jAudio[PId].Value<int>();
                audioAttachments.OwnerId = jAudio[PAttachmentOwnerId].Value<int>();
                audioAttachments.Artist = jAudio[PAudioArtist].Value<string>();
                audioAttachments.Title = jAudio[PTitle].Value<string>();
                audioAttachments.Url = jAudio[PUrl].Value<string>();
                audioAttachments.AccessKey = jAudio[PAttachmentAccessKey]?.Value<string>();

                return audioAttachments;
            }

            throw new DeserializerException("Failed recognize jObject as audio attachment", _jAudio?.ToString());
        }

        private LinkAttachment ParseLinkAttachment(JObject _jLink)
        {
            if (_jLink[PAttachmentsLink] is JObject jLink)
            {
                var linkAttachment = new LinkAttachment();

                linkAttachment.Title = jLink[PTitle].Value<string>();
                linkAttachment.Description = jLink[PLinkDescription].Value<string>();
                linkAttachment.Url = jLink[PUrl].Value<string>();
                linkAttachment.AccessKey = jLink[PAttachmentAccessKey]?.Value<string>();

                return linkAttachment;
            }

            throw new DeserializerException("Failed recognize jObject as link attachment", _jLink?.ToString());
        }
    }
}
