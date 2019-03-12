namespace BLLViews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSubscribers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Jobs", "UserOwner_Id", "dbo.AspNetUsers");
            AddColumn("dbo.Jobs", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "Job_ID", c => c.Int());
            CreateIndex("dbo.Jobs", "ApplicationUser_Id");
            CreateIndex("dbo.AspNetUsers", "Job_ID");
            AddForeignKey("dbo.AspNetUsers", "Job_ID", "dbo.Jobs", "ID");
            AddForeignKey("dbo.Jobs", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Job_ID", "dbo.Jobs");
            DropIndex("dbo.AspNetUsers", new[] { "Job_ID" });
            DropIndex("dbo.Jobs", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "Job_ID");
            DropColumn("dbo.Jobs", "ApplicationUser_Id");
            AddForeignKey("dbo.Jobs", "UserOwner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
