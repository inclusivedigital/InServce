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
    public class ValueChainsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IValueChain> Get()
        {
            var valuechains = new List<IValueChain>();
            foreach (var item in DB.ValueChains.OrderBy(c => c.Name)) valuechains.Add(item.IValueChain);
            return valuechains;
        }
    }
}
