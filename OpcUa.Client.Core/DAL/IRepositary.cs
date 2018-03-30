using System.Collections.Generic;

namespace OpcUa.Client.Core
{
    public interface IRepository
    {
        T GetById<T>(int id) where T : class, IEntity;
        T SaveRecord<T>(T record) where T : class, IEntity;
        IEnumerable<T> SaveRecords<T>(ICollection<T> record) where T : class, IEntity;
        T RemoveRecord<T>(T record) where T : class, IEntity;
        IEnumerable<T> RemoveRecords<T>(ICollection<T> records) where T : class, IEntity;
        void UpdateRecord<T>(ICollection<T> records)where T : class, IEntity;
        void UpdateRecords<T>(ICollection<T> records) where T : class, IEntity;
    }
}
