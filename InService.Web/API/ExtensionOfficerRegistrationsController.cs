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
    public class ExtensionOfficerRegistrationsController : SysController
    {
        public IExtensionOfficer Post([FromBody] IExtensionOfficer iofficer)
        {
            var officer = DB.ExtensionOfficers.FirstOrDefault(c => c.ECNumber.ToLower() == iofficer.ECNumber.ToLower());
            if (officer != null)
            {
                if (officer.UserID.HasValue)
                {
                    return new IExtensionOfficer { Firstname = "Extension offcer record already exist! Use your EC number to sign in.", ErrorCode = 1 };
                }
                else
                {
                    var user = new User
                    {
                        CreationDate = DateTime.UtcNow,
                        LoginID = iofficer.ECNumber,
                        Email = iofficer.Mobile,
                        Name = officer.Fullname,
                        StatusID = (int)UserStatus.ACTIVE,
                        RoleID = (int)UserRole.EXTENSION_OFICER,
                        Mobile = iofficer.Mobile,
                        ExtensionOfficerID = officer.ID,
                        ProvinceID = DB.Provinces.FirstOrDefault(c => c.Name == iofficer.Province)?.ID ?? null,
                        DistrictID = DB.Districts.FirstOrDefault(c => c.Name == iofficer.District)?.ID ?? null,
                    };
                    DB.Users.Add(user);
                    DB.SaveChanges();
                    officer.UserID = user.ID;
                    DB.SaveChanges();
                    return officer.IExtensionOfficer;
                }
            }
            else
            {
                return new IExtensionOfficer { Firstname = "Extension officer record not found", ErrorCode = -1 };
            }
        }
    }
}
