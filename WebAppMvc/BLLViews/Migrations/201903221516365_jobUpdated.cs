namespace BLLViews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobUpdated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Resumes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        path = c.String(),
                        TimeSend = c.DateTime(nullable: false),
                        TimeAnswer = c.DateTime(nullable: false),
                        Status = c.String(),
                        job_ID = c.Int(),
                        own_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jobs", t => t.job_ID)
                .ForeignKey("dbo.AspNetUsers", t => t.own_Id)
                .Index(t => t.job_ID)
                .Index(t => t.own_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Resumes", "own_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Resumes", "job_ID", "dbo.Jobs");
            DropIndex("dbo.Resumes", new[] { "own_Id" });
            DropIndex("dbo.Resumes", new[] { "job_ID" });
            DropTable("dbo.Resumes");
        }
    }
}
