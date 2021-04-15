using InService.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InService.Lib.Auth;

namespace InService.Data
{
    public partial class EmailConfig
    {
        public EmailTarget Target
        {
            get => (EmailTarget)TargetID;
            set => TargetID = (int)value;
        }

        public string ComputeHash(string password) => Hash = (password ?? "").GetHash(Host.ToLower().Trim());
    }
}
