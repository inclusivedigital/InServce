using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class UserExamination
    {
        public IUserExamination IUserExamination => new IUserExamination
        {
            AttachmentsJson = AttachmentsJson,
            CreationDate = CreationDate,
            ExaminationID = ExaminationID,
            UserID = UserID,
            ID = ID,
            Latitude = Latitude,
            Longitude = Longitude,
            StartTime = StartTime,
            EndTime = EndTime,
            CourseID = CourseID,
            ModuleID = ModuleID,
        };

        public FUserExamination FUserExamination => new FUserExamination
        {
            Examination = IUserExamination,
            Details = UserExaminationDetails.Select(c => c.IUserExaminationDetail).ToList(),
        };
    }
}
