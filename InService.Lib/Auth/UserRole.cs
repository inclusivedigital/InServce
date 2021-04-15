using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Auth
{
    public enum UserRole : int
    {
        EXTENSION_OFICER = 1 << 1,
        FARMER = 1 << 10,
        ADMINISTRATOR = 1 << 30,
    }
}
