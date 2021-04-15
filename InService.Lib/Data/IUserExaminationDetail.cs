using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IUserExaminationDetail
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid ExaminationID { get; set; }
        public int QuestionID { get; set; }
        public int? AnswerID { get; set; }
        public string Name { get; set; }
    }
}
