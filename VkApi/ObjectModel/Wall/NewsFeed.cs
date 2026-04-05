using System.Collections.Generic;

namespace VkApi.ObjectModel.Wall;

public class NewsFeed(IEnumerable<INewsFeedElement> elements) : List<INewsFeedElement>(elements);