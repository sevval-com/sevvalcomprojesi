using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommandResponse
    {
        /// <summary>
        /// Kullanıcı ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Silinme tarihi
        /// </summary>
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// Kurtarma son tarihi (30 gün sonra)
        /// </summary>
        public DateTime RecoveryDeadline { get; set; }

        /// <summary>
        /// Kurtarma token'ı (destek ekibine verilecek)
        /// </summary>
        public string RecoveryToken { get; set; }

    }
}
