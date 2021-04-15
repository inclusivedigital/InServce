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
    public class CurrenciesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ICurrency> Get()
        {
            var currency = new List<ICurrency>();
            foreach (var item in DB.Currencies.OrderBy(c => c.Name)) currency.Add(item.ICurrency);
            return currency;
        }
    }
}
