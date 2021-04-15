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
    public class PaymentMethodsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IPaymentMethod> Get()
        {
            var methods = new List<IPaymentMethod>();
            foreach (var item in DB.PaymentMethods.OrderBy(c => c.Name)) methods.Add(item.IPaymentMethod);
            return methods;
        }
    }
}
