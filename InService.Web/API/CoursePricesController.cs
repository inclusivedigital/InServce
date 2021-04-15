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
    public class CoursePricesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ICoursePrice> Get()
        {
            var prices = new List<ICoursePrice>();
            foreach (var item in DB.CoursePrices.OrderByDescending(c => c.CreationDate)) prices.Add(item.ICoursePrice);
            return prices;
        }
    }
}
