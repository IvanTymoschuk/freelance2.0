namespace BLLViews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Jobs", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "UserOwner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Jobs", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Jobs", new[] { "UserOwner_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Job_ID" });
            CreateTable(
                "dbo.ApplicationUserJobs",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Job_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Job_ID })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Jobs", t => t.Job_ID, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Job_ID);
            
            AddColumn("dbo.Jobs", "UserOwnerID", c => c.String());
            DropColumn("dbo.Jobs", "ApplicationUser_Id");
            DropColumn("dbo.Jobs", "UserOwner_Id");
            DropColumn("dbo.AspNetUsers", "Job_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Job_ID", c => c.Int());
            AddColumn("dbo.Jobs", "UserOwner_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Jobs", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserJobs", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.ApplicationUserJobs", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserJobs", new[] { "Job_ID" });
            DropIndex("dbo.ApplicationUserJobs", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Jobs", "UserOwnerID");
            DropTable("dbo.ApplicationUserJobs");
            CreateIndex("dbo.AspNetUsers", "Job_ID");
            CreateIndex("dbo.Jobs", "UserOwner_Id");
            CreateIndex("dbo.Jobs", "ApplicationUser_Id");
            AddForeignKey("dbo.Jobs", "UserOwner_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "Job_ID", "dbo.Jobs", "ID");
            AddForeignKey("dbo.Jobs", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
