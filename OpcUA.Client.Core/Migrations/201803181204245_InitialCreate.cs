namespace OpcUA.Client.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecordEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VariableEntityID = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VariableEntities", t => t.VariableEntityID, cascadeDelete: true)
                .Index(t => t.VariableEntityID);
            
            CreateTable(
                "dbo.RecordTimeEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArchiveTime = c.DateTime(nullable: false),
                        RecordEntity_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RecordEntities", t => t.RecordEntity_Id)
                .Index(t => t.RecordEntity_Id);
            
            CreateTable(
                "dbo.VariableEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DataType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecordEntities", "VariableEntityID", "dbo.VariableEntities");
            DropForeignKey("dbo.RecordTimeEntities", "RecordEntity_Id", "dbo.RecordEntities");
            DropIndex("dbo.RecordTimeEntities", new[] { "RecordEntity_Id" });
            DropIndex("dbo.RecordEntities", new[] { "VariableEntityID" });
            DropTable("dbo.VariableEntities");
            DropTable("dbo.RecordTimeEntities");
            DropTable("dbo.RecordEntities");
        }
    }
}
