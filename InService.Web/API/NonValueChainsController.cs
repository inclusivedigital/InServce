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
    public class NonValueChainsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<INonValueChain> Get()
        {
            var nonvaluechains = new List<INonValueChain>();
            foreach (var item in DB.NonValueChains.OrderBy(c => c.Name)) nonvaluechains.Add(item.INonValueChain);
            return nonvaluechains;
        }
    }
}
