using System.Data.Entity;
using System.Linq;

namespace OpcUa.Client.Core
{
    public class VariableRepository : BaseRepository<VariableEntity>, IVariableRepository
    {
        private DataContext DataContect => Context as DataContext;
        public VariableRepository(DataContext dataDontext): base(dataDontext) {}

        public void DeleteById(int id)
        {
            var variableToDelete = DataContect.Variables.Include(x => x.Records).SingleOrDefault(x => x.Id == id);
            if (variableToDelete == null) return;
            DataContect.Variables.Remove(variableToDelete);
            DataContect.SaveChanges();
        }
    }
}