using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXT.Models
{
    public class Group
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool TrackCost { get; set; }
        public List<User> Users { get; set; }
        public List<Record> Records { get; set; }
        public GroupIcon GroupIcon { get; set; }
    }
}
