using System;
using System.Data.Entity;

namespace OpcUa.Client.Core
{
    public class RecordRepository : Repository
    {
        public void GetRecordByDateTime(DateTime time)
        {
            using (var dbContext = new DataContext())
            {
                
            }
        }
    }
}