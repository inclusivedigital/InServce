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
    public class LivestockCategoriesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ILivestockCategory> Get()
        {
            var categories = new List<ILivestockCategory>();
            foreach (var item in DB.LivestockCategories.OrderBy(c => c.Name)) categories.Add(item.ILivestockCategory);
            return categories;
        }
    }
}
