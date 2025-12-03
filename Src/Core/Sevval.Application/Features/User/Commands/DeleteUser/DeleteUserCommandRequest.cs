using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Queries.GetUserById;

namespace Sevval.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommandRequest : IRequest<ApiResponse<DeleteUserCommandResponse>>
    {
        public const string Route = "/api/v1/account/delete";

        /// <summary>
        /// Kullanıcı ID (JWT token'dan otomatik alınır)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Kullanıcının şifresi (doğrulama için)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Onay metni - Tam olarak "HESABIMI SIL" olmalı
        /// </summary>
        public string ConfirmationText { get; set; }

        /// <summary>
        /// Silme nedeni (opsiyonel)
        /// </summary>
        public string? DeletionReason { get; set; }

    }
}
