using Microsoft.EntityFrameworkCore;
using Group1Flight.Models.DataLayer; 
using Group1Flight.Models.DataLayer.Repositories;

namespace Group1Flight.Models.DataLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected global:: Group1Flight.Models.DataLayer.FlightContext context { get; set; }
        private DbSet<T> dbSet { get; set; }

        public Repository(Group1Flight.Models.DataLayer.FlightContext ctx)
        {
            context = ctx;
            dbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> List(QueryOptions<T> options)
        {
            IQueryable<T> query = dbSet;

            foreach (string include in options.Includes) {
                query = query.Include(include);
            }

            if (options.HasWhere) {
                query = query.Where(options.Where!);
            }

            if (options.HasOrderBy) {
                query = query.OrderBy(options.OrderBy!);
            }

            return query.ToList();
        }

        public virtual T? Get(int id) => dbSet.Find(id);

        public virtual T? Get(QueryOptions<T> options)
        {
            IQueryable<T> query = dbSet;
            
            foreach (string include in options.Includes) {
                query = query.Include(include);
            }
            
            if (options.HasWhere) {
                query = query.Where(options.Where!);
            }
            
            return query.FirstOrDefault();
        }

        public virtual void Insert(T entity) => dbSet.Add(entity);
        public virtual void Update(T entity) => dbSet.Update(entity);
        public virtual void Delete(T entity) => dbSet.Remove(entity);
        public virtual void Save() => context.SaveChanges();
    } 
} 