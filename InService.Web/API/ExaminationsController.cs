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
    public class ExaminationsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IExamination> Get()
        {
            var examinations = new List<IExamination>();
            foreach (var item in DB.Examinations.OrderBy(c => c.Topic)) examinations.Add(item.IExamination);
            return examinations;
        }
    }
}
