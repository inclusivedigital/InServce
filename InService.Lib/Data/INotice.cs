using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class INotice
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public string AttachmentsJson { get; set; }
        public int CreatorID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int StatusID { get; set; }
        public int TypeID { get; set; }
        public string Data { get; set; }
        public Guid? AttachmentID { get; set; }
    }
}
