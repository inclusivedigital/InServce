using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Currency
    {
        public ICurrency ICurrency => new ICurrency
        {
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            ID = ID,
            IsDefault = IsDefault,
            Name = Name,
            Symbol = Symbol,
        };
    }
}
