
using Sevval.Persistence.Context;
using GridBox.Solar.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, new()
    {
        private readonly ApplicationDbContext dbContext;

        public WriteRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private DbSet<T> Table { get => dbContext.Set<T>(); }

        public async Task AddAsync(T Entity)
        {
            await Table.AddAsync(Entity);
        }

        public async Task AddRangeAsync(IList<T> Entities)
        {
            await Table.AddRangeAsync(Entities);
        }

        public async Task DeleteAsync(T Entity)
        {
            await Task.Run(() => Table.Remove(Entity));


        }

        public async Task DeleteRangeAsync(IList<T> Entities)
        {
            await Task.Run(() => Table.RemoveRange(Entities));

        }

        public async Task<T> UpdateAsync(T Entity)
        {
            await Task.Run(() => Table.Update(Entity));

            return Entity;
        }

        public virtual IQueryable<T> QueryableSql(string sql, params object[] parameters) => Table.FromSqlRaw(sql, parameters);
    }

}
