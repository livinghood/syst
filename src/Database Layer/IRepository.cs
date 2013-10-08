using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Layer
{
    public interface IRepository<T> where T : class
    {
        void Commit();
        void Delete(T item);
        IQueryable<T> Find();
        IList<T> FindAll();
        void Add(T item);
    }
}
