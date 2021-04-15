using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using InService.Lib.Data;

namespace InService.Web.API
{
    public class CoursesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ICourse> Get()
        {
            var courses = new List<ICourse>();
            foreach (var item in DB.Courses.OrderBy(c => c.Name)) courses.Add(item.ICourse);
            return courses;
        }
    }
}
