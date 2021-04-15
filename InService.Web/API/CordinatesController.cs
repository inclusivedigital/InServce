using InService.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace InService.Web.API
{
    public class CordinatesController : SysController
    {
        [Authorize]
        public void Post(ICordinates cordinates)
        {
            var user = CurrentUser;
            user.Latitude = (decimal)cordinates.Latitude;
            user.Longitude = (decimal)cordinates.Longitude;
            DB.SaveChanges();
        }
    }
}
