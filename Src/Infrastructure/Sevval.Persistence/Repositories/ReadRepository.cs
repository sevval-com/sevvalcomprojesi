
using Sevval.Persistence.Context;
using GridBox.Solar.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, new()
    {
        private readonly ApplicationDbContext dbContext;

        public ReadRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private DbSet<T> Table { get => dbContext.Set<T>(); }
        public IQueryable<T> Queryable() => Table;


        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate is not null) return await Table.AsNoTracking().CountAsync(predicate);

            return await Table.AsNoTracking().CountAsync();
        }


        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate, bool EnableTracking = false)
        {
            if (!EnableTracking) Table.AsNoTracking();

            return await Table.FindAsync(predicate);
        }

        public async Task<T> Find(Expression<Func<T, bool>> predicate, bool EnableTracking = false)
        {
            if (!EnableTracking) Table.AsNoTracking();

            return Table.Find(predicate);
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedEnumerable<T>>? orderBy = null, bool EnableTracking = false)
        {
            IQueryable<T> queryable = Queryable();
            if (!EnableTracking) queryable = queryable.AsNoTracking();

            if (include is not null) queryable = include(queryable);

            if (predicate is not null) queryable = queryable.Where(predicate);

            if (orderBy is not null) return orderBy(queryable).ToList();

            return await queryable.ToListAsync();
        }

        public async Task<IList<T>> GetAllByPaginationAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedEnumerable<T>>? orderBy = null, bool EnableTracking = false, int page = 1, int size = 10)
        {
            IQueryable<T> queryable = Queryable();
            if (!EnableTracking) queryable = queryable.AsNoTracking();

            if (include is not null) queryable = include(queryable);

            if (predicate is not null) queryable = queryable.Where(predicate);

            if (orderBy is not null) return orderBy(queryable).Skip((page - 1) * size).Take(size).ToList();

            return await queryable.Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool EnableTracking = false)
        {
            IQueryable<T> queryable = Queryable();
            if (!EnableTracking) queryable = queryable.AsNoTracking();

            if (include is not null) queryable = include(queryable);




            return await queryable.FirstOrDefaultAsync(predicate);
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.AnyAsync(predicate);
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return Table.Any(predicate);
        }
    }

}
