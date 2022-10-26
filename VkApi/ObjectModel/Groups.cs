using System.Collections.Generic;

namespace VkApi.ObjectModel
{
    public class Groups : List<Group>
    {
        public Groups(IEnumerable<Group> _groups) : base(_groups)
        {
            
        }
    }
}
