using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VkGrabber.DataLayer
{
    public class DbUser
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Token { get; set; }
        public virtual ICollection<DbGroup> DbGroups { get; set; }
    }
}