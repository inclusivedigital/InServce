using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IExaminationPrice
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int ExaminationID { get; set; }
        public int CurrencyID { get; set; }
        public int? PaymentMethodID { get; set; }
        public DateTime RevisionDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatorID { get; set; }
    }
}
