using System;
using System.Collections.Generic;
using System.Linq;

namespace NXTWebService.Models
{
    public class GroupDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool TrackCost { get; set; }
        public List<UserDto> Users { get; set; }
        public List<RecordDto> Records { get; set; }
        public string GroupIconName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Colour { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public String WhoseShoutDisplay
        {
            get
            {
                var user = WhoseShout;
                if (!String.IsNullOrEmpty(user.UserName))
                {
                    return user.UserName;
                }
                else
                {
                    return user.Email;
                }
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public UserDto WhoseShout
        {
            get
            {
                UserDto user = null;
                if (Users.Count > 0)
                {
                    if (Records.Count == 0)
                    {
                        return Users.First();
                    }

                    List<UserDto> tiedShout = new List<UserDto>();
                    int lowestShoutCount = int.MaxValue;
                    foreach (var u in Users)
                    {
                        if (u.RecordCount < lowestShoutCount)
                        {
                            user = u;
                            lowestShoutCount = u.RecordCount;
                            tiedShout.Clear();
                        }
                        else if (u.RecordCount == lowestShoutCount)
                        {
                            tiedShout.Add(u);
                        }
                    }

                    if (tiedShout.Count > 1)
                    {
                        foreach (var shout in Records)
                        {
                            tiedShout.Remove(tiedShout.FirstOrDefault(x => x.ID == shout.UserID));
                            if (tiedShout.Count <= 1)
                            {
                                return tiedShout[0];
                            }
                        }
                    }
                }
                return user;
            }
        }
    }

}