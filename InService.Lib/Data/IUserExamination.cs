using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IUserExamination
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid? LocalExamID { get; set; }
        public int UserID { get; set; }
        public int ExaminationID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string AttachmentsJson { get; set; }
        public DateTime? EndTime { get; set; }

        public string DetailsJson { get; set; }
        public int CourseID { get; set; }
        public int ModuleID { get; set; }
    }
}
