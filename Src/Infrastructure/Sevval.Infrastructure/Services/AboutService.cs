using GridBox.Solar.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.About.Queries.GetAboutContent;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using Sevval.Persistence.Context.sevvalemlak.Models;

namespace Sevval.Infrastructure.Services
{
    public class AboutService : IAboutService
    {

        private readonly IReadRepository<AboutUsContent>  _readRepository;

        public AboutService(IReadRepository<AboutUsContent> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<ApiResponse<GetAboutContentQueryResponse>> GetAboutContentAsync(CancellationToken cancellationToken)
        {

            var aboutUsContents = await _readRepository.Queryable().AsNoTracking().ToListAsync();


            var contents = aboutUsContents.ToDictionary(c => c.Key, c => c.Content);
 
            var response = new GetAboutContentQueryResponse
            {
                MainContent  = contents.ContainsKey("main-content") ? contents["main-content"] : string.Empty
            };


            return new ApiResponse<GetAboutContentQueryResponse>
            {
                IsSuccessfull = true,
                Data = response,
                Message = "Hakkımızda içeriği başarıyla getirildi."

            };
        }
    }
}
