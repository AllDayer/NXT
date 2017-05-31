using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NXT.Models;
using NXTWebService.Models;

namespace NXTWebService.Controllers
{
    public class GroupsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Groups
        public IQueryable<GroupDto> GetGroups()
        {
            var groups = from s in db.Groups
                              select new GroupDto()
                              {
                                  ID = s.ID,
                                  Category = s.Category,
                                  Name = s.Name,
                                  TrackCost = s.TrackCost,
                                  GroupIconIndex = s.GroupIcon.IconIndex,
                                  Users = (from u in db.NXTUsers
                                           where s.Users.Any(sh => sh.ID == u.ID)
                                           select new UserDto()
                                           {
                                               UserName = u.UserName,
                                               AvatarUrl = u.AvatarUrl,
                                               ID = u.ID
                                           }).ToList(),
                                  Records = (from record in db.Records
                                            where s.Records.Any(sh => sh.ID == record.ID)
                                            select new RecordDto()
                                            {
                                                Cost = record.Cost,
                                                ID = record.ID,
                                                PurchaseTimeUtc = record.PurchaseTimeUtc,
                                                UserName = record.User.UserName,
                                                GroupName = record.Group.Name
                                            }).ToList()

                              };
            return groups;
        }

        [ResponseType(typeof(GroupDto))]
        public IQueryable<GroupDto> GetGroupForUser(Guid userId)
        {
            var groups = from s in db.Groups.Include(sgi => sgi.GroupIcon)
                              from su in s.Users
                              where su.ID == userId
                              select new GroupDto()
                              {
                                  ID = s.ID,
                                  Category = s.Category,
                                  Name = s.Name,
                                  TrackCost = s.TrackCost,
                                  GroupIconIndex = s.GroupIcon.IconIndex,
                                  Records = (from record in db.Records
                                            where s.Records.Any(sh => sh.ID == record.ID)
                                            orderby record.PurchaseTimeUtc descending
                                            select new RecordDto()
                                            {
                                                Cost = record.Cost,
                                                ID = record.ID,
                                                PurchaseTimeUtc = record.PurchaseTimeUtc,
                                                UserName = record.User.UserName,
                                                GroupName = record.Group.Name,
                                                UserAvatarUrl = record.User.AvatarUrl
                                            }).ToList(),
                                  Users = (from u in db.NXTUsers
                                           where s.Users.Any(sh => sh.ID == u.ID)
                                           select new UserDto()
                                           {
                                               UserName = u.UserName,
                                               ID = u.ID,
                                               AvatarUrl = u.AvatarUrl,
                                               RecordCount = (from zx in db.Records
                                                             where zx.Group.ID == s.ID &&
                                                                   zx.User.ID == u.ID
                                                             select zx).Count()
                                           }).ToList()
                              };

            return groups;
        }


        // GET: api/Groups/<Guid>
        [ResponseType(typeof(GroupDto))]
        public IHttpActionResult GetGroup(Guid id)
        {
            var group = from s in db.Groups.Include(sgi => sgi.GroupIcon)
                             where s.ID == id
                             select new GroupDto()
                             {
                                 ID = s.ID,
                                 Category = s.Category,
                                 Name = s.Name,
                                 TrackCost = s.TrackCost,
                                 GroupIconIndex = s.GroupIcon != null ? s.GroupIcon.IconIndex : -1,
                                 Records = (from record in db.Records
                                           where s.Records.Any(sh => sh.ID == record.ID)
                                           orderby record.PurchaseTimeUtc descending
                                           select new RecordDto()
                                           {
                                               Cost = record.Cost,
                                               ID = record.ID,
                                               PurchaseTimeUtc = record.PurchaseTimeUtc,
                                               UserName = record.User.UserName,
                                               GroupName = record.Group.Name,
                                               UserAvatarUrl = record.User.AvatarUrl
                                           }).ToList(),
                                 Users = (from u in db.NXTUsers
                                          where s.Users.Any(sh => sh.ID == u.ID)
                                          select new UserDto()
                                          {
                                              UserName = u.UserName,
                                              ID = u.ID,
                                              AvatarUrl = u.AvatarUrl,
                                              RecordCount = (from zx in db.Records
                                                            where zx.Group.ID == s.ID &&
                                                                  zx.User.ID == u.ID
                                                            select zx).Count()
                                          }).ToList()
                             };
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // PUT: api/Groups/<guid>
        // Not tested
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGroup(Guid id, GroupDto groupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != groupDto.ID)
            {
                return BadRequest();
            }

            Group group = db.Groups.Include(sgi => sgi.GroupIcon).FirstOrDefault(x => x.ID == id);

            db.Groups.Attach(group);

            group.Name = groupDto.Name;
            group.TrackCost = groupDto.TrackCost;
            group.Category = groupDto.Category;


            if (group.GroupIcon == null)
            {
                GroupIcon sgi = new GroupIcon()
                {
                    Group = group,
                    IconIndex = groupDto.GroupIconIndex,
                };
                group.GroupIcon = sgi;
            }
            else
            {
                group.GroupIcon.IconIndex = groupDto.GroupIconIndex;
            }


            //var shoutUsersForGroup = new List<ShoutUser>();
            //foreach (var shoutGroupUser in shoutGroupDto.Users)
            //{
            //    ShoutUser foundUser = null;

            //    if (shoutGroupUser.ID != null && shoutGroupUser.ID != Guid.Empty)
            //    {
            //        foundUser = db.ShoutUsers.FirstOrDefault(x => x.ID == shoutGroupUser.ID);
            //    }
            //    else if (foundUser == null && !String.IsNullOrEmpty(shoutGroupUser.Email))
            //    {
            //        foundUser = db.ShoutUsers.FirstOrDefault(x => x.Email.Equals(shoutGroupUser.Email, StringComparison.OrdinalIgnoreCase));
            //    }

            //    if (foundUser != null)
            //    {
            //        shoutGroup.ShoutUsers.Add(foundUser);
            //        if (foundUser.ShoutGroups == null)
            //        {
            //            foundUser.ShoutGroups = new List<ShoutGroup>();
            //        }
            //        //foundUser.ShoutGroups.Add(shoutGroup);
            //        //foundUser.ShoutGroups.FirstOrDefault(x => x.ID ==)
            //        shoutUsersForGroup.Add(foundUser);
            //    }
            //    else
            //    {
            //        var newUser = new ShoutUser() { ID = Guid.NewGuid(), Email = shoutGroupUser.Email, UserName = shoutGroupUser.UserName };
            //        if (newUser.ShoutGroups == null)
            //        {
            //            newUser.ShoutGroups = new List<ShoutGroup>();
            //        }
            //        newUser.ShoutGroups.Add(shoutGroup);
            //        shoutGroup.ShoutUsers.Add(newUser);

            //        db.ShoutUsers.Add(newUser);
            //    }
            //}

            //foreach (var sh in shoutUsersForGroup)
            //{
            //    db.ShoutUsers.Attach(sh);
            //}


            db.Entry(group).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Groups
        [ResponseType(typeof(GroupDto))]
        public IHttpActionResult PostGroup(GroupDto groupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Group gro = new Group()
            {
                ID = groupDto.ID,
                Name = groupDto.Name,
                TrackCost = groupDto.TrackCost,
                Category = groupDto.Category,
                Records= new List<Record>(),
                Users = new List<User>()
            };


            var usersForGroup = new List<User>();
            foreach (var groupUser in groupDto.Users)
            {
                User foundUser = null;

                if (groupUser.ID != null && groupUser.ID != Guid.Empty)
                {
                    foundUser = db.NXTUsers.FirstOrDefault(x => x.ID == groupUser.ID);
                }
                else if (foundUser == null && !String.IsNullOrEmpty(groupUser.Email))
                {
                    foundUser = db.NXTUsers.FirstOrDefault(x => x.Email.Equals(groupUser.Email, StringComparison.OrdinalIgnoreCase));
                }

                if (foundUser != null)
                {
                    gro.Users.Add(foundUser);
                    if (foundUser.Groups == null)
                    {
                        foundUser.Groups = new List<Group>();
                    }
                    foundUser.Groups.Add(gro);
                    usersForGroup.Add(foundUser);
                }
                else
                {
                    var newUser = new User() { ID = Guid.NewGuid(), Email = groupUser.Email, UserName = groupUser.UserName };
                    if (newUser.Groups == null)
                    {
                        newUser.Groups = new List<Group>();
                    }
                    newUser.Groups.Add(gro);
                    gro.Users.Add(newUser);

                    db.NXTUsers.Add(newUser);
                }
            }

            foreach (var sh in usersForGroup)
            {
                db.NXTUsers.Attach(sh);
            }

            GroupIcon groupIcon = new GroupIcon()
            {
                GroupID = gro.ID,
                Group = gro,
                IconIndex = groupDto.GroupIconIndex
            };
            gro.GroupIcon = groupIcon;

            db.Groups.Add(gro);


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (GroupExists(groupDto.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = groupDto.ID }, groupDto);
        }

        // DELETE: api/ShoutGroups/<guid>
        [ResponseType(typeof(Group))]
        public IHttpActionResult DeleteGroup(Guid id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            db.SaveChanges();

            return Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(Guid id)
        {
            return db.Groups.Count(e => e.ID == id) > 0;
        }
    }
}