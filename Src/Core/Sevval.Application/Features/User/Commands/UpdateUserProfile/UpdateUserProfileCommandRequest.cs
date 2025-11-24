using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Commands.UpdateUser;

namespace Sevval.Application.Features.User.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandRequest : IRequest<ApiResponse<UpdateUserProfileCommandResponse>>
    {
        public const string Route = "/api/v1/user-profile";

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public bool IsDeleted { get; set; }
        public bool Status { get; set; }
        public short? CountryId { get; set; }
        public string CountryName { get; set; }
        public int? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string ParentUserId { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPersonel { get; set; }
        public string TaxNumber { get; set; }
        public string Address { get; set; }//fatura
        public string TaxOffice { get; set; }
        public short AvgStars { get; set; }
    }
}
