using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IPayment
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Reference { get; set; }
        public Guid? FarmerID { get; set; }
        public int PaymentMethodID { get; set; }
        public int CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public int StatusID { get; set; }
        public string PollURL { get; set; }

    }
}
