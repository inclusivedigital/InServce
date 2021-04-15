using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{
    public enum PaymentStatus
    {
        ACKNOWLEDGED = 1,
        CANCELED = 2,
        PENDING_ACKNOWKEDGEMENT = 3,
    }
}
