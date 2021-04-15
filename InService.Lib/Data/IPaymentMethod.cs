using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IPaymentMethod
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int CreatorID { get; set; }
        public bool RequireReference { get; set; }
        public bool IsDefault { get; set; }
    }
}
