using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.AutoMapper
{
    public interface IMapper
    {
        public TDestination Map<TDestination, TSource>(TSource source, string? ignore = null);
        public IList<TDestination> Map<TDestination, TSource>(IList<TSource> sources, string? ignore = null);
        public TDestination Map<TDestination>(object source, string? ignore = null);
        public IList<TDestination> Map<TDestination>(IList<object> source, string? ignore = null);
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }

}
