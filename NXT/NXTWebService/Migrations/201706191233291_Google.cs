namespace NXTWebService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Google : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "GoogleID", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "GoogleID");
        }
    }
}
