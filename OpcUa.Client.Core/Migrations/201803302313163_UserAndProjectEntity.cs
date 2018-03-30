namespace OpcUa.Client.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAndProjectEntity : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RecordEntities", new[] { "VariableEntityID" });
            CreateTable(
                "dbo.ProjectEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Endpoint = c.String(),
                        SessionName = c.Double(nullable: false),
                        UserEntityId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserEntities", t => t.UserEntityId)
                .Index(t => t.UserEntityId);
            
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
            
            CreateIndex("dbo.RecordEntities", "VariableEntityId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectEntities", "UserEntityId", "dbo.UserEntities");
            DropIndex("dbo.RecordEntities", new[] { "VariableEntityId" });
            DropIndex("dbo.ProjectEntities", new[] { "UserEntityId" });
            DropTable("dbo.UserEntities");
            DropTable("dbo.ProjectEntities");
            CreateIndex("dbo.RecordEntities", "VariableEntityID");
        }
    }
}
