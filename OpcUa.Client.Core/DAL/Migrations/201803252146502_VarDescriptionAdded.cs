namespace OpcUa.Client.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VarDescriptionAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VariableEntities", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VariableEntities", "Description");
        }
    }
}