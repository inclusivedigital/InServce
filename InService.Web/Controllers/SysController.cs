using InService.Data;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    [Authorize]
    public class SysController : Controller
    {
        protected readonly InServiceEntities DB = new InServiceEntities();
        protected readonly int DefaultPageSize = 1000000;
        internal int CurrentUserID => User.Identity.GetUserId<int>();

        User _currentUser;
        private readonly Random _rdm = new Random();

        internal User CurrentUser
        {
            get
            {
                if (_currentUser == null) _currentUser = DB.Users.Find(CurrentUserID);
                return _currentUser;
            }
        }

        protected string AccountGenerator(int digits)
        {
            if (digits <= 1) return "";
            var _min = (int)Math.Pow(10, digits - 1);
            var _max = (int)Math.Pow(10, digits) - 1;
            return Math.Abs(_rdm.Next(_min, _max)).ToString();
        }
    }
}