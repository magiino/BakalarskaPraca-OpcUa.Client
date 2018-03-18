namespace OpcUA.Client.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArchiveIntervalAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VariableEntities", "Archive", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VariableEntities", "Archive");
        }
    }
}
