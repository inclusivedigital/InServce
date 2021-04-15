using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class PaymentMethod
    {
        public IPaymentMethod IPaymentMethod => new IPaymentMethod
        {
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            ID = ID,
            IsDefault = IsDefault,
            Name = Name,
            RequireReference = RequireReference,
        };
    }
}
