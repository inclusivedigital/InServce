using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IExamination
    {
        [PrimaryKey]
        public int ID { get; set; }
        public Guid? LocalID { get; set; }
        public string Topic { get; set; }
        public int? CourseID { get; set; }
        public int? ModuleID { get; set; }
        public int Year { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int TypeID { get; set; }
        public decimal Duration { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int? MaxAttempts { get; set; }
        public string AttachmentsJson { get; set; }
        public int? ValueChainID { get; set; }
        public int CreatorID { get; set; }
        public int PaperFormatID { get; set; }
        public int FlagsID { get; set; }
        public int TargetAudienceID { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}
