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
    public class LivestocksController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ILivestock> Get()
        {
            var livestocks = new List<ILivestock>();
            foreach (var item in DB.Livestocks.OrderBy(c => c.Name)) livestocks.Add(item.ILivestock);
            return livestocks;
        }
    }
}
