using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;

namespace OpcUA.Client.Core
{
    public class DataContext : DbContext
    {
        #region Public Members (Tables)
        public DbSet<RecordEntity> Records { get; set; }
        public DbSet<RecordTimeEntity> RecordTimes { get; set; }
        public DbSet<VariableEntity> Variables { get; set; }

        #endregion

        #region Constructor

        public DataContext() : base("UaClientDatabase") { }

        #endregion
}
}
