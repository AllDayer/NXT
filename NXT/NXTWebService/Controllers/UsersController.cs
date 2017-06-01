using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class UsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.NXTUsers;
        }

        // GET: api/Users/5
        [ResponseType(typeof(UserDto))]
        public IHttpActionResult GetUser(Guid id)
        {
            User user = db.NXTUsers.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/Users?socialId={socialId}&authType={authType}
        [ResponseType(typeof(UserDto))]
        public IHttpActionResult GetUserBySocial(string socialId, AuthType authType)
        {
            User user = null;

            if (!String.IsNullOrEmpty(socialId))
            {
                switch (authType)
                {
                    case AuthType.Facebook:
                        user = db.NXTUsers.FirstOrDefault(x => x.FacebookID == socialId);
                        break;

                }
            }

            if (user == null)
            {
                return NotFound();
            }

            UserDto ret = new UserDto()
            {
                ID = user.ID,
                Email = user.Email,
                UserName = user.UserName,
                AuthType = AuthType.Facebook,
                SocialID = user.FacebookID,
                AvatarUrl = user.AvatarUrl,
                Colour = user.Colour
            };

            return Ok(ret);
        }

        // GET: api/Users/email={email}
        [ResponseType(typeof(UserDto))]
        public IHttpActionResult GetUserByEmail(string email)
        {
            User user = null;

            if (!String.IsNullOrEmpty(email))
            {
                user = db.NXTUsers.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PATCH: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PatchUser(Guid id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.ID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(Guid id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.ID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NXTUsers.Add(user);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user.ID }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(Guid id)
        {
            User user = db.NXTUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.NXTUsers.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(Guid id)
        {
            return db.NXTUsers.Count(e => e.ID == id) > 0;
        }
    }
}