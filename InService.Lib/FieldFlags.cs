using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{
    [Flags]
    public enum FieldFlags : int
    {
        NON = 0,
        MANDATORY = 1,
        UNIQUE = 2,
        EDITABLE = 4,
        PRINTABLE = 8,
        KEY = 16,
        HIDDEN = 32
    }
}
