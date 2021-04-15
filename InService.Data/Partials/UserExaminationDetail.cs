using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class UserExaminationDetail
    {
        public IUserExaminationDetail IUserExaminationDetail => new IUserExaminationDetail
        {
            AnswerID = AnswerID,
            ExaminationID = ExaminationID,
            ID = ID,
            Name = Name,
            QuestionID = QuestionID,
        };
    }
}
