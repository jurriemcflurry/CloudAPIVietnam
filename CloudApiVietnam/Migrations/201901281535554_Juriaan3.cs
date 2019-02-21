namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Juriaan3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FavoriteSong", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "FavoriteSong");
        }
    }
}
