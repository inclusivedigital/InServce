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
    public class BranchesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IBranch> Get()
        {
            var branches = new List<IBranch>();
            foreach (var item in DB.Branches.OrderByDescending(c => c.CreationDate)) branches.Add(item.IBranch);
            return branches;
        }
    }
}
