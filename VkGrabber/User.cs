namespace VkGrabber
{
    public class User
    {
        public string Token { get; }
        public string Key { get; }
        public Group[] Groups { get; }

        public User(string _token, string _key, Group[] _groups)
        {
            Token = _token;
            Key = _key;
            Groups = _groups;
        }
    }
}