using System.Data.Entity;

namespace OpcUA.Client.Core
{
    public class DataContext : DbContext
    {
        #region Public Members

        public DbSet<RecordEntity> Records { get; set; }
        public DbSet<RecordTimeEntity> RecordTimes { get; set; }
        public DbSet<VariableEntity> Variables { get; set; }

        #endregion

        #region Constructor

        public DataContext() : base("name=UaClientContext") { }

        #endregion
}
}
