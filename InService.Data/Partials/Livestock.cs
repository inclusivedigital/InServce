using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Livestock
    {
        public IEnumerable<Livestock> livestocks => Livestock1;

        public ILivestock ILivestock => new ILivestock
        {
            Description = Description,
            CreatorID = CreatorID,
            CreationDate = CreationDate,
            ID = ID,
            CategoryID = CategoryID,
            CourseID = CourseID,
            IconID = IconID,
            Name = Name,
            ParentID = ParentID,
        };
    }
}
