using System;
using NXT.Models;

namespace NXTWebService.Models
{
    public class UserDto
    {
        public Guid ID { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public int RecordCount { get; set; }
        public DateTime LastShoutUtc { get; set; }
        public string SocialID { get; set; }
        public AuthType AuthType { get; set; }
        public String AvatarUrl { get; set; }
        public String Colour { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}