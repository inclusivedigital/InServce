using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class LivestockCategory
    {
        public ILivestockCategory ILivestockCategory => new ILivestockCategory
        {
            BranchID = BranchID,
            ID = ID,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Description = Description,
            Name = Name,
        };
    }
}
