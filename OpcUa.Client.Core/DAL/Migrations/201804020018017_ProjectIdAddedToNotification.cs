namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectIdAddedToNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotificationEntities", "ProjectId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotificationEntities", "ProjectId");
        }
    }
}
