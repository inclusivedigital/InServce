using Microsoft.AspNet.Identity;
using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InService.Web.API
{
    [Authorize]
    public class SysController : ApiController
    {
        protected readonly InServiceEntities DB = new InServiceEntities();

        private User _CurrentUser;
        protected User CurrentUser
        {
            get
            {
                if (_CurrentUser == null) _CurrentUser = DB.Users.Find(User.Identity.GetUserId<int>());
                return _CurrentUser;
            }
        }

        protected string Path = @"C:\InService\Articles\Uploads\";
    }
}