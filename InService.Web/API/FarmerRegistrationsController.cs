using InService.Data;
using InService.Lib.Auth;
using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InService.Web.API
{
    [AllowAnonymous]
    public class FarmerRegistrationsController : SysController
    {

        public IFarmer Post([FromBody] IFarmer farmer)
        {
            var now = DateTime.UtcNow;
            var orgPrefix = "FM";
            var thisMonth = new DateTime(now.Year, now.Month, 1);
            var accNoPrefix = $"{orgPrefix}{now:yy}{now.Month:X}";
            var count = DB.Farmers.Count(c => c.CreationDate >= thisMonth);

            var item = DB.Farmers.FirstOrDefault(c => c.Name == farmer.Firstname && c.Surname == farmer.Surname && c.ID == farmer.ID);
            if (item != null)
            {

            }
            else
            {
                var user = new User
                {
                    CreationDate = DateTime.UtcNow,
                    LoginID = farmer.Mobile,
                    Email = farmer.Email,
                    Name = $"{farmer.Firstname} {farmer.Surname}",
                    StatusID = (int)UserStatus.ACTIVE,
                    RoleID = (int)UserRole.FARMER,
                    Mobile = farmer.Mobile
                };
                item = new Farmer
                {
                    ID = farmer.ID,
                    Address = farmer.Address,
                    CreationDate = DateTime.UtcNow,
                    Email = farmer.Email,
                    FarmName = farmer.Farmname,
                    GenderID = farmer.GenderID,
                    IDNumber = farmer.NationalID,
                    Mobile = farmer.Mobile,
                    Name = farmer.Firstname,
                    Surname = farmer.Surname,
                    Village = farmer.Location,
                    ProvinceID = DB.Provinces.FirstOrDefault(c => c.Name == farmer.Province)?.ID ?? null,
                    DistrictID = DB.Districts.FirstOrDefault(c => c.Name == farmer.District)?.ID ?? null,
                };
                var accNo = $"{accNoPrefix}{count:00000}";
                while (DB.Farmers.Any(c => c.Account == accNo))
                {
                    count++;
                    accNo = $"{accNoPrefix}{count:00000}";
                }
                item.Account = accNo;
                count++;
                user.FarmerID = item.ID;
                DB.Users.Add(user);
                DB.Farmers.Add(item);
            }
            DB.SaveChanges();
            return item.IFarmer;
        }
    }
}
