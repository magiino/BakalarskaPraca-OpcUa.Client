namespace OpcUa.Client.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectIdAddedToVariable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ProjectEntities", name: "UserEntityId", newName: "UserId");
            RenameColumn(table: "dbo.RecordEntities", name: "VariableEntityId", newName: "VariableId");
            RenameIndex(table: "dbo.ProjectEntities", name: "IX_UserEntityId", newName: "IX_UserId");
            RenameIndex(table: "dbo.RecordEntities", name: "IX_VariableEntityId", newName: "IX_VariableId");
            AddColumn("dbo.VariableEntities", "ProjectId", c => c.Int(nullable: false));
            AlterColumn("dbo.ProjectEntities", "SessionName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProjectEntities", "SessionName", c => c.Double(nullable: false));
            DropColumn("dbo.VariableEntities", "ProjectId");
            RenameIndex(table: "dbo.RecordEntities", name: "IX_VariableId", newName: "IX_VariableEntityId");
            RenameIndex(table: "dbo.ProjectEntities", name: "IX_UserId", newName: "IX_UserEntityId");
            RenameColumn(table: "dbo.RecordEntities", name: "VariableId", newName: "VariableEntityId");
            RenameColumn(table: "dbo.ProjectEntities", name: "UserId", newName: "UserEntityId");
        }
    }
}
