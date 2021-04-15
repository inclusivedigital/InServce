using InService.Lib;
using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class ExtensionOfficer
    {
        public string Fullname => string.Join(" ", new string[] { Firstname, Surname, }.Where(c => !string.IsNullOrWhiteSpace(c)));
        public Gender Gender
        {
            get => (Gender)(GenderID ?? 1);
            set => GenderID = (int)value;
        }
        public IExtensionOfficer IExtensionOfficer => new IExtensionOfficer
        {
            CreationDate = CreationDate,
            ECNumber = ECNumber,
            Firstname = Firstname,
            ID = ID,
            IsSynced = true,
            NationalID = NationalID,
            Surname = Surname,
            UserID = UserID,
            Email = Email,
            Mobile = Mobile,
            District = District?.Name ?? "",
            GenderID = GenderID,
            Province = Province?.Name ?? "",
            DateOfBirth = DateOfBirth
        };
        public string Initials
        {
            get
            {
                var firstChars = Fullname.Where((ch, index) => ch != ' '
                       && (index == 0 || Fullname[index - 1] == ' '));
                return string.Join("", firstChars).ToUpper();
            }
        }
    }
}
