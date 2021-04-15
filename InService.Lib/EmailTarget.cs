using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{

    [Flags]
    public enum EmailTarget : int
    {
        REGISTRATION_CONFIRMATION = 1,
        INVOICES = 2,
        RECEIPTS = 4,
        BULK_COMMUNICATION = 8,
    }
}
