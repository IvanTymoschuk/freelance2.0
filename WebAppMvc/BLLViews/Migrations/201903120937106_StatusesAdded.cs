namespace BLLViews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        toDate = c.DateTime(nullable: false),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.user_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountStatus", "user_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AccountStatus", new[] { "user_Id" });
            DropTable("dbo.AccountStatus");
        }
    }
}
