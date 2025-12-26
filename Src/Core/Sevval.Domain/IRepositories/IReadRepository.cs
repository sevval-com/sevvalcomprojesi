using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GridBox.Solar.Domain.IRepositories
{
    public interface IReadRepository<T> where T : class, new()
    {
        Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedEnumerable<T>>? orderBy = null,
            bool EnableTracking = false
            );


        Task<IList<T>> GetAllByPaginationAsync(
           Expression<Func<T, bool>>? predicate = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
           Func<IQueryable<T>, IOrderedEnumerable<T>>? orderBy = null,
           bool EnableTracking = false,
           int page = 1, int size = 10
           );


        Task<T> GetAsync(
          Expression<Func<T, bool>> predicate,
          Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
         bool EnableTracking = false
          );



        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<T> FindAsync(Expression<Func<T, bool>> predicate, bool EnableTracking = false);
        Task<T> Find(Expression<Func<T, bool>> predicate, bool EnableTracking = false);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        IQueryable<T> Queryable();
    }

}
