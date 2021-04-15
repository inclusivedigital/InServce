using InService.Lib;
using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Instruction
    {
        public ExaminationType Type
        {
            get => (ExaminationType)ExamTypeID;
            set => ExamTypeID = (int)value;
        }

        public IInstruction IInstruction => new IInstruction
        {
            ID = ID,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Description = Description,
            ExamTypeID = ExamTypeID,
        };

    }
}
