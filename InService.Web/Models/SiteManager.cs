using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InService.Web.Models
{
    public static class SiteManager
    {
        static InServiceEntities DB = new InServiceEntities();

        public static IEnumerable<Course> Courses
        {
            get => DB.Courses.OrderBy(c => c.Name);
        }
        public static IEnumerable<ValueChain> ValueChains
        {
            get => DB.ValueChains.OrderBy(c => c.Name);
        }
        public static IEnumerable<Module> Modules
        {
            get => DB.Modules.OrderBy(c => c.Name);
        }
        public static IEnumerable<Branch> Branches 
        {
            get => DB.Branches.OrderBy(c => c.Name);
        }
    }
}