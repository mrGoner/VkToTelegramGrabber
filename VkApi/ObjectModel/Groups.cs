using System.Collections.Generic;

namespace VkApi.ObjectModel;

public class Groups(IEnumerable<Group> groups) : List<Group>(groups);