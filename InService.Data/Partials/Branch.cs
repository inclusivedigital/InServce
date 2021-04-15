using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Branch
    {
        public IBranch IBranch => new IBranch
        {
            CreationDate = CreationDate,
            ID = ID,
            Description = Description,
            CreatorID = CreatorID,
            IconID = IconID,
            Name = Name,
            ParentID = ParentID,
            SectionID = SectionID
        };
    }
}
