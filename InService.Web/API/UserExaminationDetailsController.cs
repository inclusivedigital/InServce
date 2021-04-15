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
    public class UserExaminationDetailsController : SysController
    {
        [Authorize]
        public IEnumerable<IUserExaminationDetail> Get()
        {
            var details = new List<IUserExaminationDetail>();
            var items = CurrentUser.UserExaminations.SelectMany(c => c.UserExaminationDetails);
            foreach (var item in items) details.Add(item.IUserExaminationDetail);
            return details;
        }

    }
}
