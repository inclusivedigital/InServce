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
    public class CropsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ICrop> Get()
        {
            var crops = new List<ICrop>();
            foreach (var item in DB.Crops.OrderBy(c => c.Name)) crops.Add(item.ICrop);
            return crops;
        }
    }
}
