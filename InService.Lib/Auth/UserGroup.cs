using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Auth
{
    public enum UserGroup
    {
        CEO = 1 << 1,
        FINANCE_MANAGER = 1 << 2,
        EXTENSION_MANAGER = 1 << 3,
        M_AND_E_OFFICER = 1 << 4,
        BUSINESS_DEVELOPMENT_OFFICER = 1 << 5,
        FIELD_EXTENSION_OFFICER = 1 << 6,
        MCC_ADMIN = 1 << 7,
        WAREHOUSE_MANAGER = 1 << 8,
        CASHIER = 1 << 9,
    }
}
