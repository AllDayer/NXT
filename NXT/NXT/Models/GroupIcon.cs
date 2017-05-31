using System;
using System.Collections.Generic;
#if WebService
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXT.Models
{
    public class GroupIcon
    {
#if WebService
        [Key, ForeignKey("Group")]
#endif
        public Guid GroupID { get; set; }
        public int IconIndex { get; set; }

        public Group Group { get; set; }
    }
}
