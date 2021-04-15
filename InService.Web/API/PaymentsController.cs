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
    public class PaymentsController : SysController
    {
        [Authorize]
        public IEnumerable<IPayment> Get()
        {
            var payments = new List<IPayment>();
            var query = DB.Payments.OrderByDescending(c=>c.CreationDate).AsQueryable();
            if (CurrentUser != null)
            {
                if (CurrentUser.FarmerID.HasValue) query = query.Where(c => c.FarmerID == CurrentUser.FarmerID);
                else query = query.Where(c => c.CreatorID == CurrentUser.ID);
            }
            foreach (var item in query) payments.Add(item.IPayment);
            return payments;
        }
    }
}

