using System;
using System.Data.Entity.Migrations;
using Opc.Ua;

namespace OpcUa.Client.Core.DAL
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DAL\Migrations";
            MigrationsNamespace = "OpcUa.Client.Core.DAL.Migrations";
        }

        protected override void Seed(DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var record1 = new RecordEntity()
            {
                Id = 1,
                VariableId = 1,
                Value = "566",
                ArchiveTime = DateTime.Now
            };
            context.Records.AddOrUpdate(x => x.Id, record1);

            var record2 = new RecordEntity()
            {
                Id = 2,
                VariableId = 2,
                Value = "20",
                ArchiveTime = DateTime.Now
            };
            context.Records.AddOrUpdate(x => x.Id, record2);

            var record3 = new RecordEntity()
            {
                Id = 3,
                VariableId = 3,
                Value = "1500",
                ArchiveTime = DateTime.Now
            };
            context.Records.AddOrUpdate(x => x.Id, record3);

            var record4 = new RecordEntity()
            {
                Id = 4,
                VariableId = 1,
                Value = "576",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(30)
            };
            context.Records.AddOrUpdate(x => x.Id, record4);

            var record5 = new RecordEntity()
            {
                Id = 5,
                VariableId = 2,
                Value = "26",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(30)
            };
            context.Records.AddOrUpdate(x => x.Id, record5);

            var record6 = new RecordEntity()
            {
                Id = 6,
                VariableId = 3,
                Value = "1566",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(30)
            };
            context.Records.AddOrUpdate(x => x.Id, record6);

            var record7 = new RecordEntity()
            {
                Id = 7,
                VariableId = 1,
                Value = "586",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(60)
            };
            context.Records.AddOrUpdate(x => x.Id, record7);

            var record8 = new RecordEntity()
            {
                Id = 8,
                VariableId = 2,
                Value = "33",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(60)
            };
            context.Records.AddOrUpdate(x => x.Id, record8);

            var record9 = new RecordEntity()
            {
                Id = 9,
                VariableId = 3,
                Value = "1570",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(60)
            };
            context.Records.AddOrUpdate(x => x.Id, record9);

            var var1 = new VariableEntity()
            {
                Id = 1,
                ProjectId = 1,
                Name = "ns=2;s=Demo.Dynamic.Scalar.Int16",
                Description = "Test Int16 Value",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = BuiltInType.Int16,
            };
            context.Variables.AddOrUpdate(x => x.Id, var1);

            var var2 = new VariableEntity()
            {
                Id = 2,
                ProjectId = 1,
                Name = "ns=2;s=Demo.Dynamic.Scalar.SByte",
                Description = "Test Short Byte Value",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = BuiltInType.SByte,
            };
            context.Variables.AddOrUpdate(x => x.Id, var2);

            var var3 = new VariableEntity()
            {
                Id = 3,
                ProjectId = 1,
                Name = "ns=2;s=Demo.Dynamic.Scalar.UInt64",
                Description = "Test Unsigned Int64 Value",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = BuiltInType.UInt64,
            };
            context.Variables.AddOrUpdate(x => x.Id, var3);

            var project = new ProjectEntity()
            {
                Id = 1,
                Name = "TestProject",
                Endpoint = "opc.tcp://A05-226b:48010",
                UserId = null,
                SessionName = "TestProjectSession"
            };
            context.Projects.AddOrUpdate(x => x.Id, project);
        }
    }
}
