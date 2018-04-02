namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndpointEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EndpointEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        SecurityPolicyUri = c.String(),
                        MessageSecurityMode = c.Int(nullable: false),
                        TransportProfileUri = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ProjectEntities", "EndpointId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProjectEntities", "EndpointId");
            AddForeignKey("dbo.ProjectEntities", "EndpointId", "dbo.EndpointEntities", "Id", cascadeDelete: true);
            DropColumn("dbo.ProjectEntities", "Endpoint");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectEntities", "Endpoint", c => c.String());
            DropForeignKey("dbo.ProjectEntities", "EndpointId", "dbo.EndpointEntities");
            DropIndex("dbo.ProjectEntities", new[] { "EndpointId" });
            DropColumn("dbo.ProjectEntities", "EndpointId");
            DropTable("dbo.EndpointEntities");
        }
    }
}
