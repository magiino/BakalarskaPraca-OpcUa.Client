namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
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
            
            CreateTable(
                "dbo.NotificationEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Guid(nullable: false),
                        Name = c.String(),
                        NodeId = c.String(),
                        FilterValue = c.Double(nullable: false),
                        DeadbandType = c.Int(nullable: false),
                        IsDigital = c.Boolean(nullable: false),
                        IsZeroDescription = c.String(),
                        IsOneDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectEntities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        EndpointId = c.Int(nullable: false),
                        SessionName = c.String(),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EndpointEntities", t => t.EndpointId, cascadeDelete: true)
                .ForeignKey("dbo.UserEntities", t => t.UserId)
                .Index(t => t.EndpointId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        PasswordHash = c.Binary(),
                        PasswordSalt = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RecordEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VariableId = c.Int(nullable: false),
                        Value = c.String(),
                        ArchiveTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VariableEntities", t => t.VariableId, cascadeDelete: true)
                .Index(t => t.VariableId);
            
            CreateTable(
                "dbo.VariableEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        DataType = c.Int(nullable: false),
                        Archive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecordEntities", "VariableId", "dbo.VariableEntities");
            DropForeignKey("dbo.ProjectEntities", "UserId", "dbo.UserEntities");
            DropForeignKey("dbo.ProjectEntities", "EndpointId", "dbo.EndpointEntities");
            DropIndex("dbo.RecordEntities", new[] { "VariableId" });
            DropIndex("dbo.ProjectEntities", new[] { "UserId" });
            DropIndex("dbo.ProjectEntities", new[] { "EndpointId" });
            DropTable("dbo.VariableEntities");
            DropTable("dbo.RecordEntities");
            DropTable("dbo.UserEntities");
            DropTable("dbo.ProjectEntities");
            DropTable("dbo.NotificationEntities");
            DropTable("dbo.EndpointEntities");
        }
    }
}
