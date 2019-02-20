using System;

namespace VkTools.Authorization
{
    public static class AuthHelper
    {
        private static string m_urlTemplate = "https://oauth.vk.com/authorize?client_id={0}&display=page&redirect_uri=https://oauth.vk.com/blank.html&response_type=token&v={1}&scope={2}";

        public static string BuildAuthString(int _applicationId, Permissions _permissions, string _apiVersion)
        {
            if (string.IsNullOrWhiteSpace(_apiVersion))
                throw new ArgumentException("Api version can not be null or white space!");

            var scope = "";

            if (_permissions.HasFlag(Permissions.Friends))
                scope += "friends,";
            if (_permissions.HasFlag(Permissions.Photos))
                scope += "photos,";
            if (_permissions.HasFlag(Permissions.Audio))
                scope += "audio,";
            if (_permissions.HasFlag(Permissions.Video))
                scope += "video,";
            if (_permissions.HasFlag(Permissions.Messages))
                scope += "messages,";
            if (_permissions.HasFlag(Permissions.Offline))
                scope += "offline,";
            if (_permissions.HasFlag(Permissions.Groups))
                scope += "groups,";
            if (_permissions.HasFlag(Permissions.Docs))
                scope += "docs,";
            if (_permissions.HasFlag(Permissions.Wall))
                scope += "wall";

            scope.TrimEnd(',');

            var url = string.Format(m_urlTemplate, _applicationId, _apiVersion, scope);

            return url;
        }
    }
}
