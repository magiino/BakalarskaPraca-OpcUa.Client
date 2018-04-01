namespace OpcUa.Client.Core.DAL.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UserAndProjectEntityAdded : DbMigration
    {
        public override void Up()
        {
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
            
            CreateTable(
                "dbo.RecordEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VariableEntityId = c.Int(nullable: false),
                        Value = c.String(),
                        ArchiveTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VariableEntities", t => t.VariableEntityId, cascadeDelete: true)
                .Index(t => t.VariableEntityId);
            
            CreateTable(
                "dbo.VariableEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DataType = c.Int(nullable: false),
                        Archive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecordEntities", "VariableEntityId", "dbo.VariableEntities");
            DropForeignKey("dbo.ProjectEntities", "UserEntityId", "dbo.UserEntities");
            DropIndex("dbo.RecordEntities", new[] { "VariableEntityId" });
            DropIndex("dbo.ProjectEntities", new[] { "UserEntityId" });
            DropTable("dbo.VariableEntities");
            DropTable("dbo.RecordEntities");
            DropTable("dbo.UserEntities");
            DropTable("dbo.ProjectEntities");
        }
    }
}
