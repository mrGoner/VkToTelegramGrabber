﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string PAudioArtist = "artist";
        public const string PLinkDescription = "description";
        public const string PAttachmentAccessKey = "access_key";
        public const string PCopyHistrory = "copy_history";
        public const string PHistoryOwnerId = "owner_id";
        public const string PHistoryFromId = "from_id";
        public const string PSourceId = "source_id";
        public const string PVideoImage = "image";
        public const string PVideoFirstFrame = "first_frame";

        #endregion

        private VideoAttachmentDeserializer m_videoAttachmentDeserializer = new VideoAttachmentDeserializer();

        public NewsFeed Deserialize(string _data, Func<VideoInfo, string> _loadVideoItem = null)
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
                            newsFeed.Add(ParsePostItem(jItem, _loadVideoItem));
                    }

                    return newsFeed;
                }
                catch (Exception ex)
                {
                    throw new DeserializerException("Failed to deserialize newsfeed", ex);
                }
            }

            throw new DeserializerException($"Failed recognize jObject as vk response \n {_data}");
        }

        private Post ParsePostItem(JObject _jPostItem, Func<VideoInfo, string> _loadVideoItem)
        {
            try
            {
                var post = new Post();

                post.SourceId = _jPostItem[PSourceId].Value<int>();
                post.Date = EpochTimeConverter.ConvertToDateTime(_jPostItem[PItemDate].Value<long>());
                post.PostId = _jPostItem[PItemId].Value<int>();
                post.Text = _jPostItem[PItemText].Value<string>();
                post.SignerId = _jPostItem[PItemSignerId]?.Value<int>() ?? null;
                post.MarkedAsAds = _jPostItem[PItemMarkedAsAds].Value<int>() != 0;
                post.PostSource = ParsePostSource((JObject)_jPostItem[PPostSource]);

                var attachmentsRaw = _jPostItem[PAttachments];
                if (attachmentsRaw != null)
                    post.Attachments = ParseAttachments(attachmentsRaw, _loadVideoItem).ToArray();

                post.Comments = ParseComments((JObject)_jPostItem[PComments]);
                post.Likes = ParseLikes((JObject)_jPostItem[PLikes]);
                post.Reposts = ParseReposts((JObject)_jPostItem[PReposts]);
                post.Views = ParseViews((JObject)_jPostItem[PViews]);

                var rawHistoryElem = _jPostItem[PCopyHistrory];

                if (rawHistoryElem != null)
                    post.CopyHistory = ParseHistory(rawHistoryElem, _loadVideoItem).ToArray();

                return post;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse post item \n {_jPostItem?.ToString()}", ex);
            }
        }

        private List<HistoryPost> ParseHistory(JToken _jHistory, Func<VideoInfo, string> _loadVideoItem)
        {
            if (_jHistory is JArray jCopyHistory)
            {
                try
                {
                    var historyCollection = new List<HistoryPost>();

                    foreach (JObject jHistoryElement in jCopyHistory)
                    {
                        var historyPost = new HistoryPost();

                        historyPost.Id = jHistoryElement[PId].Value<int>();
                        historyPost.OwnerId = jHistoryElement[PHistoryOwnerId].Value<int>();
                        historyPost.FromId = jHistoryElement[PHistoryFromId].Value<int>();
                        historyPost.Date = EpochTimeConverter.ConvertToDateTime(jHistoryElement[PDate].Value<long>());
                        historyPost.Text = jHistoryElement[PItemText].Value<string>();
                        historyPost.PostSource = ParsePostSource((JObject)jHistoryElement[PPostSource]);

                        var attachmentsRaw = jHistoryElement[PAttachments];

                        if (attachmentsRaw != null)
                            historyPost.Attachments = ParseAttachments(attachmentsRaw, _loadVideoItem).ToArray();

                        historyCollection.Add(historyPost);

                    }

                    return historyCollection;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to parse history \n {_jHistory.ToString()}", ex);
                }
            }

            throw new ArgumentException($"History element not recognized as array \n {_jHistory?.ToString()}");
        }

        private Comments ParseComments(JObject _jComments)
        {
            try
            {
                int count = _jComments[PCommentsCount].Value<int>();
                bool canComment = _jComments[PCommentsCanPost].Value<int>() != 0;
                var canGroupsCommentRaw = _jComments[PCommentsGroupsCanPost]?.Value<int>();

                bool? groupCanComment = canGroupsCommentRaw == null ? null : new bool?(canGroupsCommentRaw != 0);

                return new Comments(count, canComment, groupCanComment);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse comments \n {_jComments.ToString()}", ex);
            }
        }

        private PostSource ParsePostSource(JObject _jPostSource)
        {
            try
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
                        postSource.Type = PostSourceType.Unknown;
                        break;
                }

                var rawPlatformType = _jPostSource[PPostSourcePlatform]?.Value<string>();

                if (rawPlatformType != null)
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
                            postSource.Platfrom = PlatformType.Unknown;
                            break;
                    }

                    if (postSource.Type == PostSourceType.Widget || postSource.Type == PostSourceType.Vk)
                    {
                        postSource.Data = _jPostSource[PPostSourceData]?.Value<string>();
                    }
                }

                postSource.Url = _jPostSource[PId]?.Value<string>();

                return postSource;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse post source \n {_jPostSource.ToString()}", ex);
            }
        }

        private Likes ParseLikes(JObject _jLikes)
        {
            try
            {
                var count = _jLikes[PLikesCount].Value<int>();
                bool userLikes = _jLikes[PLikesUserLikes].Value<int>() != 0;
                bool canLike = _jLikes[PLikesCanLike].Value<int>() != 0;
                bool canPublish = _jLikes[PLikesCanPublish].Value<int>() != 0;

                return new Likes(count, userLikes, canLike, canPublish);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse likes \n {_jLikes.ToString()}", ex);
            }
        }

        private Reposts ParseReposts(JObject _jReposts)
        {
            try
            {
                int count = _jReposts[PRepostsCount].Value<int>();
                bool userReposted = _jReposts[PRepostsUserReposted].Value<int>() != 0;

                return new Reposts(count, userReposted);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse reposts \n {_jReposts.ToString()}", ex);
            }
        }

        private Views ParseViews(JObject _jViews)
        {
            try
            {
                var count = _jViews[PViewsCount].Value<int>();

                return new Views(count);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse views \n {_jViews.ToString()}", ex);
            }
        }

        private List<IAttachmentElement> ParseAttachments(JToken _jAttachments, Func<VideoInfo, string> _loadVideoItem)
        {
            try
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
                                attachments.Add(ParseVideoAttachment(jAttachment, _loadVideoItem));
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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse attachments \n {_jAttachments.ToString()}", ex);
            }

            throw new ArgumentException($"Failed to recognize attachment token as jArray \n {_jAttachments?.ToString()}");
        }

        private PhotoAttachment ParsePhotoAttachment(JObject _jPhoto)
        {
            try
            {
                if (_jPhoto[PAttachmentsPhoto] is JObject photoJObj)
                {
                    var photoAttachment = new PhotoAttachment();

                    photoAttachment.Id = photoJObj[PId].Value<int>();
                    photoAttachment.AlbumId = photoJObj[PPhotoAlbumId].Value<int>();
                    photoAttachment.OwnerId = photoJObj[PAttachmentOwnerId].Value<int>();
                    photoAttachment.UserId = photoJObj[PPhotoUserId]?.Value<int>();
                    photoAttachment.Text = photoJObj[PPhotoText]?.Value<string>();
                    photoAttachment.Date = EpochTimeConverter.ConvertToDateTime(photoJObj[PDate].Value<long>());
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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse photo attachment \n {_jPhoto.ToString()}", ex);
            }

            throw new ArgumentException($"Failed recognize jObject as photo attachment \n {_jPhoto?.ToString()}");
        }

        private DocumentAttachment ParseDocAttachment(JObject _jDoc)
        {
            try
            {
                if (_jDoc[PAttachmentsDocument] is JObject jDoc)
                {
                    var docAttachment = new DocumentAttachment();

                    docAttachment.Id = jDoc[PId].Value<int>();
                    docAttachment.OwnerId = jDoc[PAttachmentOwnerId].Value<int>();
                    docAttachment.Title = jDoc[PTitle]?.Value<string>();
                    docAttachment.Date = EpochTimeConverter.ConvertToDateTime(jDoc[PDate].Value<long>());
                    docAttachment.Url = jDoc[PUrl].Value<string>();
                    docAttachment.AccessKey = jDoc[PAttachmentAccessKey]?.Value<string>();

                    return docAttachment;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse doc attachment \n {_jDoc.ToString()}", ex);
            }

            throw new DeserializerException($"Failed recognize jObject as document attachment \n {_jDoc?.ToString()}");
        }

        private VideoAttachment ParseVideoAttachment(JObject _jVideo, Func<VideoInfo, string> _loadVideoItem)
        {
            try
            {
                if (_jVideo[PAttachmentsVideo] is JObject jVideo)
                {
                    var videoAttachment = new VideoAttachment
                    {
                        Id = jVideo[PId].Value<int>(),
                        OwnerId = jVideo[PAttachmentOwnerId].Value<int>(),
                        Title = jVideo[PTitle].Value<string>(),
                        Description = jVideo[PVideoDescription].Value<string>(),
                        Duration = jVideo[PVideoDuration].Value<int>(),
                        Date = EpochTimeConverter.ConvertToDateTime(jVideo[PDate].Value<long>()),
                        Views = jVideo[PVideoViews].Value<int>(),
                        CommentsCount = jVideo[PVideoComments]?.Value<int>(),
                        PlayerUrl = jVideo[PVideoPlayer]?.Value<string>(),
                        AccessKey = jVideo[PAttachmentAccessKey].Value<string>()
                    };


                    if (jVideo.ContainsKey(PVideoImage) && jVideo[PVideoImage] is JArray jImages)
                    {
                        videoAttachment.Images = jImages.Select(_x =>
                        {
                            return new Image
                            {
                                Height = _x[PSizesHeight].Value<int>(),
                                Width = _x[PSizesWidth].Value<int>(),
                                Url = _x[PUrl].Value<string>()
                            };
                        }).ToArray();
                    }

                    if (jVideo.ContainsKey(PVideoFirstFrame) && jVideo[PVideoFirstFrame] is JArray jFrames)
                    {
                        videoAttachment.FirstFrames = jFrames.Select(_x =>
                        {
                            return new Image
                            {
                                Height = _x[PSizesHeight].Value<int>(),
                                Width = _x[PSizesWidth].Value<int>(),
                                Url = _x[PUrl].Value<string>()
                            };
                        }).ToArray();
                    }

                    if(videoAttachment.PlayerUrl == null && _loadVideoItem != null)
                    {
                        try
                        {
                            var data = _loadVideoItem(new VideoInfo
                            {
                                OwnerId = videoAttachment.OwnerId,
                                VideoId = videoAttachment.Id
                            });

                            var videoInfo = m_videoAttachmentDeserializer.Deserialize(data);

                            videoAttachment.PlayerUrl = videoInfo.PlayerUrl;
                        }
                        catch
                        {
                            //nothing to do
                        }
                    }

                    return videoAttachment;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse video attachment \n {_jVideo.ToString()}", ex);
            }

            throw new ArgumentException($"Failed recognize jObject as video attachment \n {_jVideo?.ToString()}");
        }

        private AudioAttachment ParseAudioAttachment(JObject _jAudio)
        {
            try
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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse audio attachment \n {_jAudio.ToString()}", ex);
            }

            throw new ArgumentException($"Failed recognize jObject as audio attachment \n {_jAudio?.ToString()}");
        }

        private LinkAttachment ParseLinkAttachment(JObject _jLink)
        {
            try
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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse link attachment \n {_jLink.ToString()}", ex);
            }

            throw new ArgumentException($"Failed recognize jObject as link attachment \n {_jLink?.ToString()}");
        }
    }
}