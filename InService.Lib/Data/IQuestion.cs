using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IQuestion
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int ExaminationID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int Score { get; set; }
        public string AttachmentsJson { get; set; }
        public int CreatorID { get; set; }
    }
}
