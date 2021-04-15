using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class ICrop
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public int CategoryID { get; set; }
        public int? CourseID { get; set; }
        public Guid? IconID { get; set; }
    }
}
