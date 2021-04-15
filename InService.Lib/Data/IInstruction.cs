using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IInstruction
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int ExamTypeID { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        public System.DateTime CreationDate { get; set; }
    }
}
