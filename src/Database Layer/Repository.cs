using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Configuration;

namespace Database_Layer
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext db = new DataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);

        /// <summary>
        /// Add the changes to the database
        /// </summary>
        public void Commit()
        {
            db.SubmitChanges();
        }

        /// <summary>
        /// Delete an item from the database
        /// </summary>
        /// <param name="item"></param>
        public void Delete(T item)
        {
            var table = this.LookupTableFor(typeof(T));

            table.DeleteOnSubmit(item);
        }

        /// <summary>
        /// Find all items in the database and use extra filtering
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Find()
        {
            var table = this.LookupTableFor(typeof(T));

            return table.Cast<T>();
        }

        /// <summary>
        /// Get all the entites of the type T
        /// </summary>
        /// <returns></returns>
        public IList<T> FindAll()
        {
            var table = this.LookupTableFor(typeof(T));

            return table.Cast<T>().ToList();
        }

        /// <summary>
        /// Add item to database
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var table = this.LookupTableFor(typeof(T));

            table.InsertOnSubmit(item);
        }

        private ITable LookupTableFor(Type entityType)
        {
            return db.GetTable(entityType);
        }
    }
}
