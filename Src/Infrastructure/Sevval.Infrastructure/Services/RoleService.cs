using AutoMapper;
using GridBox.Solar.Domain.IRepositories;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Role.Queries.GetUserRoles;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using System.Net;

namespace Sevval.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IWriteRepository<Role> _writeRepository;
        private readonly IReadRepository<Role> _readRepository;
        private readonly IMapper _mapper;

        public RoleService(IWriteRepository<Role> writeRepository, IReadRepository<Role> readRepository, IMapper mapper)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _mapper = mapper;
        }


        public async Task<ApiResponse<IList<GetUserRolesQueryResponse>>> GetUserRoles(GetUserRolesQueryRequest request)
        {
            var roles = await _readRepository.GetAllAsync();

            return new ApiResponse<IList<GetUserRolesQueryResponse>>
            {
                Code = (int)HttpStatusCode.OK,
                Data = roles.Select(a => new GetUserRolesQueryResponse() { Name = a.Name }).ToList(),
                IsSuccessfull = true,
                Message = "Kayıt Getirildi"
            };
        }

    }
}
