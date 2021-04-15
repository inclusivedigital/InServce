using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class CropCategory
    {
        public ICropCategory ICropCategory => new ICropCategory
        {
            Name = Name,
            IconID = IconID,
            Description = Description,
            CreatorID = CreatorID,
            CreationDate = CreationDate,
            BranchID = BranchID,
            ID = ID,
        };
    }
}
