using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class ICourse
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public int BranchID { get; set; }
        public int? ValueChainID { get; set; }
        public int? NonValueChainID { get; set; }
        public string Glossary { get; set; }
        public string AttachmentsJson { get; set; }
        public Guid? IconID { get; set; }
        public int? FinalExamQuestions { get; set; }
    }
}
