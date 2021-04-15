
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
    public class ExaminationPricesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IExaminationPrice> Get()
        {
            var prices = new List<IExaminationPrice>();
            foreach (var item in DB.ExaminationPrices.OrderByDescending(c => c.CreationDate)) prices.Add(item.IExaminationPrice);
            return prices;
        }
    }
}
