namespace VkTools.ObjectModel
{
    public class Group
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public string ScreenName { get; internal set; }
        public int IsClosed { get; internal set; }
        public GroupType Type { get; internal set; }
        public bool IsAdmin { get; internal set; }
        public bool IsMember { get; internal set; }
        public bool IsAdvertiser { get; internal set; }
        public string PhotoSmall { get; internal set; }
        public string PhotoMedium { get; internal set; }
        public string PhotoLarge { get; internal set; }
    }
}