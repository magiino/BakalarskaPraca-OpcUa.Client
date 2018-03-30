using System.Collections.Generic;
using System.Linq;

namespace OpcUa.Client.Core
{
    // TODO osetrit vynimky ci sa spravne pridali/zmazali data
    // TODO vzdy vytvarat nove connection? alebo pouzivat vzdy to jedno?
    // TODO aky je rozdiel ak mam jedno connection a vraciam tasky oproti viac connections bez taskov
    // TODO osetrit vstupne hodnoty
    // TODO update
    // TODO ak pridavam premennu s tým istým názvom tak ju nepridávat !!!
    public abstract class Repository : IRepository
    {
        protected Repository() {}

        public T GetById<T>(int id) 
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
                return dbContext.Set<T>().SingleOrDefault(x => x.Id == id);
        }

        public T SaveRecord<T>(T record) 
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
            {
                var savedRecord = dbContext.Set<T>().Add(record);
                dbContext.SaveChanges();
                return savedRecord;
            }
        }

        public IEnumerable<T> SaveRecords<T>(ICollection<T> records)
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
            {
                var savedRecords = dbContext.Set<T>().AddRange(records);
                dbContext.SaveChanges();
                return savedRecords;
            }
        }

        public T RemoveRecord<T>(T record)
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
            {
                var removedRecord = dbContext.Set<T>().Remove(record);
                dbContext.SaveChanges();
                return removedRecord;
            }
        }

        public IEnumerable<T> RemoveRecords<T>(ICollection<T> records)
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
            {
                var removedRecords = dbContext.Set<T>().RemoveRange(records);
                dbContext.SaveChanges();
                return removedRecords;
            }
        }

        public void UpdateRecord<T>(ICollection<T> records)
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
            {
            }
        }

        public void UpdateRecords<T>(ICollection<T> records)
            where T : class, IEntity
        {
            using (var dbContext = new DataContext())
            {
            }
        }
    }
}
