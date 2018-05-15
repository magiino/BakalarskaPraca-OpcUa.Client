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
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContext context)
        {
            // Records
            var record1 = new RecordEntity()
            {
                Id = 1,
                VariableId = 1,
                Value = "566",
                ArchiveTime = DateTime.Now
            };

            var record2 = new RecordEntity()
            {
                Id = 2,
                VariableId = 2,
                Value = "20",
                ArchiveTime = DateTime.Now
            };

            var record3 = new RecordEntity()
            {
                Id = 3,
                VariableId = 3,
                Value = "1500",
                ArchiveTime = DateTime.Now
            };

            var record4 = new RecordEntity()
            {
                Id = 4,
                VariableId = 1,
                Value = "576",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(30)
            };

            var record5 = new RecordEntity()
            {
                Id = 5,
                VariableId = 2,
                Value = "26",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(30)
            };

            var record6 = new RecordEntity()
            {
                Id = 6,
                VariableId = 3,
                Value = "1566",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(30)
            };

            var record7 = new RecordEntity()
            {
                Id = 7,
                VariableId = 1,
                Value = "586",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(60)
            };

            var record8 = new RecordEntity()
            {
                Id = 8,
                VariableId = 2,
                Value = "33",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(60)
            };

            var record9 = new RecordEntity()
            {
                Id = 9,
                VariableId = 3,
                Value = "1570",
                ArchiveTime = DateTime.Now + TimeSpan.FromSeconds(60)
            };
            context.Records.AddOrUpdate(x => x.Id, record1, record2, record3, record4, record5, record6, record7, record8, record9);

            // Variables
            var var1 = new VariableEntity()
            {
                Id = 1,
                ProjectId = new Guid("db93cbda-e293-41db-ad54-bee5a4c254a3"),
                Name = "ns=2;s=Demo.Dynamic.Scalar.Int16",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = BuiltInType.Int16,
            };

            var var2 = new VariableEntity()
            {
                Id = 2,
                ProjectId = new Guid("db93cbda-e293-41db-ad54-bee5a4c254a3"),
                Name = "ns=2;s=Demo.Dynamic.Scalar.SByte",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = BuiltInType.SByte,
            };

            var var3 = new VariableEntity()
            {
                Id = 3,
                ProjectId = new Guid("db93cbda-e293-41db-ad54-bee5a4c254a3"),
                Name = "ns=2;s=Demo.Dynamic.Scalar.UInt64",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = BuiltInType.UInt64,
            };
            context.Variables.AddOrUpdate(x => x.Id, var1, var2, var3);


            var endpoint = new EndpointEntity()
            {
                Id = 1,
                Url = "opc.tcp://A05-226b:48010",
                MessageSecurityMode = MessageSecurityMode.None,
                SecurityPolicyUri = SecurityPolicies.None,
                TransportProfileUri = Profiles.UaTcpTransport
            };
            context.Endpoints.AddOrUpdate(x => x.Id, endpoint);


            var project = new ProjectEntity()
            {
                Id = new Guid("db93cbda-e293-41db-ad54-bee5a4c254a3"),
                Name = "TestProject",
                EndpointId = 1,
                UserId = null,
                SessionName = "TestProjectSession"
            };
            context.Projects.AddOrUpdate(x => x.Id, project);

            context.SaveChanges();
        }
    }
}
