﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{
    [Flags]
    public enum ArticleFlags
    {
        PUBLISHED = 1,
        UNPUBLISHED = 2,
        DELETED = 3,
    }
}
