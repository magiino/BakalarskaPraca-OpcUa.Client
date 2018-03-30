namespace OpcUa.Client.Core.Migrations
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
                        ArchiveTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VariableEntities", t => t.VariableEntityID, cascadeDelete: true)
                .Index(t => t.VariableEntityID);
            
            CreateTable(
                "dbo.VariableEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DataType = c.Int(nullable: false),
                        Archive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecordEntities", "VariableEntityID", "dbo.VariableEntities");
            DropIndex("dbo.RecordEntities", new[] { "VariableEntityID" });
            DropTable("dbo.VariableEntities");
            DropTable("dbo.RecordEntities");
        }
    }
}
