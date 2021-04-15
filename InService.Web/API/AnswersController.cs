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
    public class AnswersController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IAnswer> Get()
        {
            var answers = new List<IAnswer>();
            foreach (var item in DB.Answers.OrderBy(c => c.Name)) answers.Add(item.IAnswer);
            return answers;
        }
    }
}
