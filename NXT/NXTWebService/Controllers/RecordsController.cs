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
    public class RecordsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Shouts
        public IQueryable<RecordDto> GetRecords()
        {
            //return db.Shouts.Include(g => g.ShoutGroup);

            var records = from s in db.Records
                         select new RecordDto()
                         {
                             ID = s.ID,
                             PurchaseTimeUtc = s.PurchaseTimeUtc,
                             Cost = s.Cost,
                             Category = s.Group.Category,
                             GroupID = s.GroupID,
                             GroupName = s.Group.Name,
                             UserID = s.User.ID,
                             UserName = s.User.UserName,
                         };
            return records;
        }

        // GET: api/Shouts/5
        [ResponseType(typeof(Record))]
        public IHttpActionResult GetRecord(Guid id)
        {
            //Shout shout = db.Shouts.Find(id);

            var record = from s in db.Records
                        where s.ID == id
                        select new RecordDto()
                        {
                            ID = s.ID,
                            PurchaseTimeUtc = s.PurchaseTimeUtc,
                            Cost = s.Cost,
                            Category = s.Group.Category,
                            GroupID = s.GroupID,
                            GroupName = s.Group.Name,
                            UserID = s.User.ID,
                            UserName = s.User.UserName,
                        };
            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);
        }

        public IQueryable<RecordDto> RecordsForGroup(Guid groupID)
        {
            //return db.Shouts.Include(g => g.ShoutGroup);

            var records = from s in db.Records
                         where s.GroupID == groupID
                         select new RecordDto()
                         {
                             ID = s.ID,
                             PurchaseTimeUtc = s.PurchaseTimeUtc,
                             Cost = s.Cost,
                             Category = s.Group.Category,
                             GroupID = s.GroupID,
                             GroupName = s.Group.Name,
                             UserID = s.User.ID,
                             UserName = s.User.UserName,
                         };
            return records;
        }


        // PUT: api/Shouts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRecord(Guid id, Record record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != record.ID)
            {
                return BadRequest();
            }

            db.Entry(record).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists(id))
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

        // POST: api/Records
        [ResponseType(typeof(RecordDto))]
        public IHttpActionResult PostShout(RecordDto recordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Record record = new Record()
            {
                ID = recordDto.ID,
                Cost = recordDto.Cost,
                PurchaseTimeUtc = DateTime.UtcNow,

                Group = db.Groups.FirstOrDefault(sg => sg.ID == recordDto.GroupID),
                User = db.NXTUsers.FirstOrDefault(su => su.ID == recordDto.UserID)
            };

            //Verify the user is in the group?

            db.Records.Add(record);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RecordExists(record.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = recordDto.ID }, recordDto);
        }

        // DELETE: api/Records/5
        [ResponseType(typeof(Record))]
        public IHttpActionResult DeleteShout(Guid id)
        {
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return NotFound();
            }

            db.Records.Remove(record);
            db.SaveChanges();

            return Ok(record);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecordExists(Guid id)
        {
            return db.Records.Count(e => e.ID == id) > 0;
        }
    }
}