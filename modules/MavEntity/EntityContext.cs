using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Dynamic;
using System.Linq.Dynamic;
using Maverick;

namespace MavEntity
{
    public class EntityContext : DataContext
    {
        public EntityContext(string connectionStr) : base(connectionStr)
        {
            if (!DatabaseExists())
            {
                throw new InvalidOperationException("Database does not exist.");
            }            
        }       

        public IQueryable<dynamic> this[string entityType]
        {
            get
            {                
                return (IQueryable<dynamic>)GetTable(ServiceLocator.GetType(entityType));
            }
        }
        
        public void Add(object obj)
        {
            GetTable(obj.GetType()).InsertOnSubmit(obj);                        
        }

        public void Remove(object obj)
        {
            GetTable(obj.GetType()).DeleteOnSubmit(obj);            
        }                
    }    
        
}
