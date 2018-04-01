using System.Data.Entity.Migrations;

namespace OpcUa.Client.Core.DAL.Migrations
{
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
