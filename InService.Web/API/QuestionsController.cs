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
    public class QuestionsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IQuestion> Get()
        {
            var questions = new List<IQuestion>();
            foreach (var item in DB.Questions.OrderBy(c => c.Name)) questions.Add(item.IQuestion);
            return questions;
        }
    }
}
