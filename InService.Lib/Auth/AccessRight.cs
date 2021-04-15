using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Auth
{
    [Flags]
    public enum AccessRight
    {
        MANAGE_USERS = 1 << 0,
        MANAGE_EXAMINATIONS = 1 << 1,

    }
}
