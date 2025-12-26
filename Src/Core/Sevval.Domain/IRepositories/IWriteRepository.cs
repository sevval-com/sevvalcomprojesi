using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridBox.Solar.Domain.IRepositories
{
    public interface IWriteRepository<T> where T : class, new()
    {
        Task AddAsync(T Entity);

        Task AddRangeAsync(IList<T> Entities);

        Task<T> UpdateAsync(T Entity);

        Task DeleteAsync(T Entity);
        Task DeleteRangeAsync(IList<T> Entities);
        IQueryable<T> QueryableSql(string sql, params object[] parameters);
    }
}
