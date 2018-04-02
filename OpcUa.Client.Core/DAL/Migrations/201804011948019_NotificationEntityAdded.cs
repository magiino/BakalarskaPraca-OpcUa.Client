namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NodeId = c.String(),
                        FilterValue = c.Double(nullable: false),
                        DeadbandType = c.Int(nullable: false),
                        IsDigital = c.Boolean(nullable: false),
                        IsZeroDescription = c.String(),
                        IsOneDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NotificationEntities");
        }
    }
}
