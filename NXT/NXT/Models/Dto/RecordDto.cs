using System;
using System.Collections.Generic;
using System.Linq;

namespace NXTWebService.Models
{
    public class RecordDto
    {
        public Guid ID { get; set; }
        public Guid GroupID { get; set; }
        public Guid UserID { get; set; }
        public DateTime PurchaseTimeUtc { get; set; }
        public float Cost { get; set; }
        public String GroupName { get; set; }
        public String Category { get; set; }
        public String UserName { get; set; }
        public String UserAvatarUrl { get; set; }
    }    
}