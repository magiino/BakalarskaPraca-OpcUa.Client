using System.Data.Entity;

namespace OpcUa.Client.Core
{
    public class DataContext : DbContext
    {
        #region Public Members

        public DbSet<RecordEntity> Records { get; set; }
        public DbSet<VariableEntity> Variables { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<EndpointEntity> Endpoints { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        #endregion

        #region Constructor

        public DataContext() : base("name=UaClientContext") { }

        #endregion
    }
}
