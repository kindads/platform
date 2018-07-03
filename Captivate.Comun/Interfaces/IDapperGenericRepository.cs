using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Interfaces
{
    public interface IDapperGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        T FindById<TI>(TI id);
    }
}
