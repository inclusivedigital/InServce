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
    public class CropCategoriesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<ICropCategory> Get()
        {
            var categories = new List<ICropCategory>();
            foreach (var item in DB.CropCategories.OrderBy(c => c.Name)) categories.Add(item.ICropCategory);
            return categories;
        }
    }
}
