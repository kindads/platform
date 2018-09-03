using System;
using System.Collections.Generic;
using System.Text;

namespace KindAds.Repository
{
    public interface IRepository<T> where T : class
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);


        T GetById(int id);
        IEnumerable<T> List();
        IEnumerable<T> List(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        
    }

    public abstract class EntityBase
    {
        public int Id { get; protected set; }
    }
}
