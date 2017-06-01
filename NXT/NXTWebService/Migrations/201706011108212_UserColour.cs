namespace NXTWebService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserColour : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Colour", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Colour");
        }
    }
}
