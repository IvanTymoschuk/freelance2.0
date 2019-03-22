namespace BLLViews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class figjfdigi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ResumePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ResumePath");
        }
    }
}
