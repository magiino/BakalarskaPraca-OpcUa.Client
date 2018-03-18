using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpcUA.Client.Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OpcUA.Client.Core.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OpcUA.Client.Core.DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var record1 = new RecordEntity()
            {
                Id = 1,
                VariableEntityID = 1,
                Value = "566",
                ArchiveTime = DateTime.Now
            };
            context.Records.AddOrUpdate(x => x.Id, record1);

            var record2 = new RecordEntity()
            {
                Id = 2,
                VariableEntityID = 2,
                Value = "20",
                ArchiveTime = DateTime.Now
            };
            context.Records.AddOrUpdate(x => x.Id, record2);

            var record3 = new RecordEntity()
            {
                Id = 3,
                VariableEntityID = 3,
                Value = "1500",
                ArchiveTime = DateTime.Now
            };
            context.Records.AddOrUpdate(x => x.Id, record3);

            var record4 = new RecordEntity()
            {
                Id = 4,
                VariableEntityID = 1,
                Value = "576",
                ArchiveTime = DateTime.Now + TimeSpan.FromMinutes(1)
            };
            context.Records.AddOrUpdate(x => x.Id, record4);

            var record5 = new RecordEntity()
            {
                Id = 5,
                VariableEntityID = 2,
                Value = "26",
                ArchiveTime = DateTime.Now + TimeSpan.FromMinutes(1)
            };
            context.Records.AddOrUpdate(x => x.Id, record5);

            var record6 = new RecordEntity()
            {
                Id = 6,
                VariableEntityID = 3,
                Value = "1566",
                ArchiveTime = DateTime.Now + TimeSpan.FromMinutes(1)
            };
            context.Records.AddOrUpdate(x => x.Id, record6);

            var record7 = new RecordEntity()
            {
                Id = 7,
                VariableEntityID = 1,
                Value = "586",
                ArchiveTime = DateTime.Now + TimeSpan.FromMinutes(2)
            };
            context.Records.AddOrUpdate(x => x.Id, record7);

            var record8 = new RecordEntity()
            {
                Id = 8,
                VariableEntityID = 2,
                Value = "33",
                ArchiveTime = DateTime.Now + TimeSpan.FromMinutes(2)
            };
            context.Records.AddOrUpdate(x => x.Id, record8);

            var record9 = new RecordEntity()
            {
                Id = 9,
                VariableEntityID = 3,
                Value = "1570",
                ArchiveTime = DateTime.Now + TimeSpan.FromMinutes(2)
            };
            context.Records.AddOrUpdate(x => x.Id, record9);

            var var1 = new VariableEntity()
            {
                Id = 1,
                Name = "ns=2;s=Demo.Dynamic.Scalar.Int16",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = "Int16",
            };
            context.Variables.AddOrUpdate(x => x.Id, var1);

            var var2 = new VariableEntity()
            {
                Id = 2,
                Name = "ns=2;s=Demo.Dynamic.Scalar.SByte",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = "SByte",
            };
            context.Variables.AddOrUpdate(x => x.Id, var2);

            var var3 = new VariableEntity()
            {
                Id = 3,
                Name = "ns=2;s=Demo.Dynamic.Scalar.UInt64",
                Archive = ArchiveInterval.ThirtySecond,
                DataType = "UInt64",
            };
            context.Variables.AddOrUpdate(x => x.Id, var3);
        }
    }
}
