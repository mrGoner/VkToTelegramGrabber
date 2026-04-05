namespace VkGrabber;

public class User
{
    public string Token { get; }
    public string Key { get; }
    public Group[] Groups { get; }

    public User(string token, string key, Group[] groups)
    {
        Token = token;
        Key = key;
        Groups = groups;
    }
}