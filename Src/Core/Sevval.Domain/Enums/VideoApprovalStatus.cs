using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Enums
{
    /// <summary>
    /// Video onay durumunu temsil eden enum
    /// </summary>
    public enum VideoApprovalStatus
    {
        /// <summary>
        /// Onay bekliyor
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Onaylandı ve yayında
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Reddedildi
        /// </summary>
        Rejected = 2
    }
}
