using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class ICoursePrice
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyID { get; set; }
        public int? PaymentMethodID { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime RevisionDate { get; set; }

    }
}
