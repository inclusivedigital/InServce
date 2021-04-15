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
    public class ModulesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IModule> Get()
        {
            var modules = new List<IModule>();
            foreach (var item in DB.Modules.OrderBy(c => c.Name)) modules.Add(item.IModule);
            return modules;
        }
    }
}
