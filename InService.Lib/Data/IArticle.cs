using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IArticle
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int? ModuleID { get; set; }
        public int? CourseID { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public int? ValueChainID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int FlagsID { get; set; }
        public bool IsDefault { get; set; }
        public string AttachmentsJson { get; set; }
        public string Data { get; set; }
        public Guid? AttachmentID { get; set; }
    }
}
