using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IAnswer
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public int QuestionID { get; set; }
        public int CreatorID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string AttachmentsJson { get; set; }
        public int FlagsID { get; set; }
        public string Comments { get; set; }

    }
}
