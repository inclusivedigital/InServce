using InService.Lib;
using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Payment
    {
        public PaymentStatus Status
        {
            get => (PaymentStatus)StatusID;
            set => StatusID = (int)value;
        }

        public IPayment IPayment => new IPayment
        {
            Amount = Amount,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            CurrencyID = CurrencyID,
            FarmerID = FarmerID,
            ID = ID,
            PaymentMethodID = PaymentMethodID,
            PollURL = PollURL,
            Reference = Reference,
            StatusID = StatusID,

        };
    }
}
