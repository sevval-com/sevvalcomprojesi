using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sevval.Application.Utilities
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalItems = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageIndex,
            int pageSize,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            pageIndex = pageIndex > 0 ? pageIndex : 1;
            pageSize = pageSize > 0 ? pageSize : 20;

            // --- Güvenlik: sıralama kontrolü
            if (!source.Expression.ToString().Contains("OrderBy"))
            {
                throw new InvalidOperationException("Pagination öncesinde mutlaka OrderBy uygulanmalı!");
            }

            var count = await source.CountAsync();

            if (include is not null)
            {
                source = include(source);
            }

            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
