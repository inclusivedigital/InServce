using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class FUserExamination
    {
        public IUserExamination Examination { get; set; }
        public List<IUserExaminationDetail> Details { get; set; }
    }
}
