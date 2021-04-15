using InService.Lib;
using InService.Lib.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class User
    {
        public Gender Gender
        {
            get => (Gender)(GenderID ?? 1);
            set => GenderID = (int)value;
        }
        public UserRole Role
        {
            get => (UserRole)RoleID;
            set => RoleID = (int)value;
        }

        public UserStatus Status
        {
            get => (UserStatus)StatusID;
            set => StatusID = (int)value;
        }

        public AccessRight AccessRight
        {
            get => (AccessRight)(AccessRightID ?? 1);
            set => AccessRightID = (int)value;
        }
        public bool IsInRole(UserRole role) => (RoleID & (int)role) == (int)role;
        public decimal Balance => Payments.Select(c => c.Amount).DefaultIfEmpty(0).Sum() - UserDeductions.Select(c => c.Amount).DefaultIfEmpty(0).Sum();


    }
}
