using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class ILivestockCategory
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public int? BranchID { get; set; }
    }
}
