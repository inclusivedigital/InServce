using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IValueChain
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public int? ParentID { get; set; }
        public int? BranchID { get; set; }
    }
}
