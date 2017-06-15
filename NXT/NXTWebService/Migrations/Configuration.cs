namespace NXTWebService.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using NXT.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<NXTWebService.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NXTWebService.Models.ApplicationDbContext context)
        {
            var record = new Record
            {
                ID = new Guid("45def82e-2b68-47e1-98c6-7d34906b46f1"),
                GroupID = new Guid("bf3641d1-a384-494d-a957-18f2aa42170c"),
                UserID = new Guid("d9c91004-3994-4bb4-a703-267904985126"),
                Cost = 9.0f,
                PurchaseTimeUtc = DateTime.UtcNow
            };

            var user1 = new User
            {
                ID = new Guid("d9c91004-3994-4bb4-a703-267904985126"),
                UserName = "Tristan",
                Email = "t@t.com",
                FacebookID = "11111111111111111",
                AvatarUrl = "http://localhost.com",
                Groups = new List<Group>(),
                Records = new List<Record>()
            };
            var user2 = new User
            {
                ID = new Guid("c9c9f88b-853b-46e5-a70a-fad212fab6b0"),
                UserName = "Norman",
                Email = "n@n.com",
                TwitterID = "12519262411111111111",
                AvatarUrl = "http://localhost.com",
                Groups = new List<Group>(),
                Records = new List<Record>()
            };
            var user3 = new User
            {
                ID = new Guid("840a9916-ca86-4575-9025-6adb2abfa389"),
                UserName = "Elspeth",
                Email = "elspethelf@hotmail.com",
                FacebookID = "11111111111111112",
                AvatarUrl = "http://localhost.com",
                Groups = new List<Group>(),
                Records = new List<Record>()
            };

            var group = new Group
            {
                ID = new Guid("bf3641d1-a384-494d-a957-18f2aa42170c"),
                Category = "Coffee",
                Name = "CoffeeTime",
                TrackCost = true,
                Users = new List<User>(),
                Records = new List<Record>()
            };

            var group2 = new Group
            {
                ID = new Guid("9befd37c-62c6-4fcf-9d77-8945c7964e7b"),
                Category = "Beer",
                Name = "Beeeeers",
                TrackCost = false,
                Users = new List<User>(),
                Records = new List<Record>()
            };

            GroupIcon icon = new GroupIcon()
            {
                GroupID = group2.ID,
                Group = group2
            };

            record.User = user1;
            record.Group = group;

            user1.Groups.Add(group);
            user2.Groups.Add(group);

            user1.Records.Add(record);

            group.Users.Add(user1);
            group.Users.Add(user2);
            group.Records.Add(record);

            group2.Users.Add(user3);

            context.Records.AddOrUpdate(p => p.ID, record);
            context.NXTUsers.AddOrUpdate(p => p.ID, user1, user2, user3);
            context.Groups.AddOrUpdate(p => p.ID, group, group2);
        }
    }
}
