using System;
using Newtonsoft.Json.Linq;
using VkTools.ObjectModel;

namespace VkTools.Serializers
{
    public class GroupsDeserializer
    {
        #region consts

        public const string PResponse = "response";
        public const string PItems = "items";
        public const string PId = "id";
        public const string PName = "name";
        public const string PScreenName = "screen_name";
        public const string PIsClosed = "is_closed";
        public const string PType = "type";
        public const string PIsAdmin = "is_admin";
        public const string PIsMember = "is_member";
        public const string PIsAdvertiser = "is_advertiser";
        public const string PPhotoSmall = "photo_50";
        public const string PPhotoMedium = "photo_100";
        public const string PPhotoLarge = "photo_200";

        #endregion

        public Groups Deserialize(string _data)
        {
            var jObject = JObject.Parse(_data);

            if (jObject[PResponse][PItems] is JArray jGroups)
            {
                try
                {
                    var groups = new Groups();

                    foreach (JObject jGroup in jGroups)
                    {
                        var group = new Group();

                        group.Id = jGroup[PId].Value<int>();
                        group.Name = jGroup[PName].Value<string>();
                        group.ScreenName = jGroup[PScreenName].Value<string>();
                        group.IsClosed = jGroup[PIsClosed].Value<int>();
                        group.IsAdmin = jGroup[PIsAdmin].Value<int>() != 0;
                        group.IsMember = jGroup[PIsMember].Value<int>() != 0;
                        group.IsAdvertiser = jGroup[PIsAdvertiser].Value<int>() != 0;
                        group.PhotoSmall = jGroup[PPhotoSmall].Value<string>();
                        group.PhotoMedium = jGroup[PPhotoMedium].Value<string>();
                        group.PhotoLarge = jGroup[PPhotoLarge].Value<string>();

                        var rawGroupType = jGroup[PType].Value<string>();

                        switch (rawGroupType)
                        {
                            case "group":
                                group.Type = GroupType.Group;
                                break;
                            case "page":
                                group.Type = GroupType.Page;
                                break;
                            case "event":
                                group.Type = GroupType.Event;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"{rawGroupType} out of range!");
                        }

                        groups.Add(group);
                    }

                    return groups;
                }
                catch (Exception ex)
                {
                    throw new DeserializerException($"Failed to deserialize groups /n {jGroups.ToString()}", ex);
                }
            }

            throw new DeserializerException($"Failed parse data as group response! /n {_data}");
        }
    }
}
