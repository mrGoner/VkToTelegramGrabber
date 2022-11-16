using System.Collections.Generic;

namespace VkApi.ObjectModel.Wall
{
    public class NewsFeed : List<INewsFeedElement> 
    {
        public NewsFeed(IEnumerable<INewsFeedElement> elements) : base(elements)
        {
            
        }
    }
}