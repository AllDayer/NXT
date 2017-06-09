using System;
using System.Collections.Generic;

#if WebService
using System.ComponentModel.DataAnnotations;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXT.Models
{
    public class User
    {
        public Guid ID { get; set; }
#if WebService
        [MaxLength(128)]
#endif
        public String UserName { get; set; }
#if WebService
        [MaxLength(256)]
#endif
        public String Email { get; set; }
#if WebService
        [MaxLength(256)]
#endif
        public String FacebookID { get; set; }//10158583186595237
#if WebService
        [MaxLength(256)]
#endif
        public String TwitterID { get; set; } //125192624
#if WebService
        [MaxLength(512)]
#endif
        public String AvatarUrl { get; set; }

#if WebService
        [MaxLength(16)]
#endif
        public String Colour { get; set; }

        public List<Group> Groups { get; set; }
        public List<Record> Records { get; set; }
    }
}
