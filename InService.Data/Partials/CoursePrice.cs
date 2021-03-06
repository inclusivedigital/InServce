using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class CoursePrice
    {
        public ICoursePrice ICoursePrice => new ICoursePrice
        {
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Amount = Amount,
            CourseID = CourseID,
            CurrencyID = CurrencyID,
            ID = ID,
            PaymentMethodID = PaymentMethodID,
            RevisionDate = RevisionDate,
        };
    }
}
