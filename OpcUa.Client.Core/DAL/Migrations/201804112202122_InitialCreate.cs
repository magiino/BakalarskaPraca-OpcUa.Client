namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EndpointEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(nullable: false),
                        SecurityPolicyUri = c.String(nullable: false),
                        MessageSecurityMode = c.Int(nullable: false),
                        TransportProfileUri = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotificationEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        NodeId = c.String(nullable: false, maxLength: 50),
                        FilterValue = c.Double(nullable: false),
                        DeadbandType = c.Int(nullable: false),
                        IsDigital = c.Boolean(nullable: false),
                        IsZeroDescription = c.String(maxLength: 50),
                        IsOneDescription = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.NodeId, unique: true);
            
            CreateTable(
                "dbo.ProjectEntities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        EndpointId = c.Int(nullable: false),
                        SessionName = c.String(nullable: false),
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
                        UserName = c.String(nullable: false),
                        PasswordHash = c.Binary(nullable: false),
                        PasswordSalt = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RecordEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VariableId = c.Int(nullable: false),
                        Value = c.String(nullable: false),
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
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        DataType = c.Int(nullable: false),
                        Archive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecordEntities", "VariableId", "dbo.VariableEntities");
            DropForeignKey("dbo.ProjectEntities", "UserId", "dbo.UserEntities");
            DropForeignKey("dbo.ProjectEntities", "EndpointId", "dbo.EndpointEntities");
            DropIndex("dbo.VariableEntities", new[] { "Name" });
            DropIndex("dbo.RecordEntities", new[] { "VariableId" });
            DropIndex("dbo.ProjectEntities", new[] { "UserId" });
            DropIndex("dbo.ProjectEntities", new[] { "EndpointId" });
            DropIndex("dbo.NotificationEntities", new[] { "NodeId" });
            DropTable("dbo.VariableEntities");
            DropTable("dbo.RecordEntities");
            DropTable("dbo.UserEntities");
            DropTable("dbo.ProjectEntities");
            DropTable("dbo.NotificationEntities");
            DropTable("dbo.EndpointEntities");
        }
    }
}
