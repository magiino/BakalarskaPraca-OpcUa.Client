namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DescriptionDeleted : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VariableEntities", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VariableEntities", "Description", c => c.String());
        }
    }
}
