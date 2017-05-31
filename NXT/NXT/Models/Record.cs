using System;

namespace NXT.Models
{
    public class Record
    {
        public Guid ID { get; set; }
        public Guid GroupID { get; set; }
        public Guid UserID { get; set; }
        public DateTime PurchaseTimeUtc { get; set; }
        public float Cost { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }
    }
}
