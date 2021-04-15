using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class ExaminationPrice
    {

        public IExaminationPrice IExaminationPrice => new IExaminationPrice
        {
            Amount = Amount,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            CurrencyID = CurrencyID,
            ExaminationID = ExaminationID,
            ID = ID,
            PaymentMethodID = PaymentMethodID,
            RevisionDate = RevisionDate,
        };
    }

}
