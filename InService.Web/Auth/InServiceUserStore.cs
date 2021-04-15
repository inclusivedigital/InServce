using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using InService.Data;
using InService.Lib.Auth;

namespace InService.Web.Auth
{
    public class InServiceIUserStore :
        IUserStore<InServiceIUser>,
        IUserRoleStore<InServiceIUser, string>,
        IUserLockoutStore<InServiceIUser, string>,
        IUserPasswordStore<InServiceIUser>,
        IUserTwoFactorStore<InServiceIUser, string>
    {
        InServiceEntities DB = new InServiceEntities();

        public InServiceIUserStore()
        {
            if (DB.Users.Count() == 0)
            {
                DB.Users.Add(new User
                {
                    CreationDate = DateTime.UtcNow,
                    LoginID = "admin",
                    Email = "mapikuw@gmail.com",
                    Name = "Wellington",
                    StatusID = (int)UserStatus.ACTIVE,
                    RoleID = (int)UserRole.ADMINISTRATOR,
                    Mobile = "0775534461",
                });
                DB.SaveChanges();
            }
        }

        public Task AddToRoleAsync(InServiceIUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(InServiceIUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(InServiceIUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            DB.Dispose();
        }

        public Task<InServiceIUser> FindByIdAsync(string userId)
        {
            var user = DB.Users.Find(int.Parse(userId));
            return user == null ? null : Task.FromResult(new InServiceIUser(user));
        }

        public Task<InServiceIUser> FindByNameAsync(string userName)
        {
            var user = DB.Users.FirstOrDefault(c => c.LoginID.ToLower() == userName);
            return user == null ? Task.FromResult<InServiceIUser>(null) : Task.FromResult(new InServiceIUser(user));
        }

        public Task<int> GetAccessFailedCountAsync(InServiceIUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(InServiceIUser user)
        {
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(InServiceIUser user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(InServiceIUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(InServiceIUser user)
        {
            var roles = new List<string>();
            foreach (UserRole item in Enum.GetValues(typeof(UserRole)))
            {
                if (user.User.IsInRole(item)) roles.Add(item.ToString());
            }
            return Task.FromResult((IList<string>)roles);
        }

        public Task<bool> GetTwoFactorEnabledAsync(InServiceIUser user)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HasPasswordAsync(InServiceIUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(InServiceIUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(InServiceIUser user, string roleName)
        {
            var role = (UserRole)Enum.Parse(typeof(UserRole), roleName);
            return Task.FromResult(user.User.IsInRole(role));
        }

        public Task RemoveFromRoleAsync(InServiceIUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(InServiceIUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(InServiceIUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(InServiceIUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(InServiceIUser user, string passwordHash)
            => Task.Factory.StartNew(() => user.PasswordHash = passwordHash);

        public Task SetTwoFactorEnabledAsync(InServiceIUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(InServiceIUser user)
        {
            return DB.SaveChangesAsync();
        }
    }
}