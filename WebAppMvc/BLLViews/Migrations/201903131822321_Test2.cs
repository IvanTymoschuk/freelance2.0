namespace BLLViews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "UserOwner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Jobs", "UserOwner_Id");
            AddForeignKey("dbo.Jobs", "UserOwner_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Jobs", "UserOwnerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "UserOwnerID", c => c.String());
            DropForeignKey("dbo.Jobs", "UserOwner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Jobs", new[] { "UserOwner_Id" });
            DropColumn("dbo.Jobs", "UserOwner_Id");
        }
    }
}
