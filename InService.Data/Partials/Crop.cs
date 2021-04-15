using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Crop
    {
        public ICrop ICrop => new ICrop
        {
            ID = ID,
            CourseID = CourseID,
            CategoryID = CategoryID,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Description = Description,
            IconID = IconID,
            Name = Name,
        };
    }
}
