using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.Base
{
    public interface IBaseInterface<TResponse, TRequest>
    {
        public Task<List<TResponse>> GetAllAsync(TRequest request);
        public Task<TResponse> GetByIdAsync(Guid id);
        public Task<TResponse> AddAsync(TRequest request);
    }

}
