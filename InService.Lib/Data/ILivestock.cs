using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class ILivestock
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> CourseID { get; set; }
        public Nullable<System.Guid> IconID { get; set; }
    }
}
