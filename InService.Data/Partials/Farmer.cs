using InService.Lib;
using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Farmer
    {
        public Gender Gender
        {
            get => (Gender)GenderID;
            set => GenderID = (int)value;
        }
        public FarmerType Type
        {
            get => (FarmerType)(TypeID ?? 0);
            set => TypeID = (int)value;
        }
        public string Fullname => string.Join(" ", new string[] { Name, Surname, }.Where(c => !string.IsNullOrWhiteSpace(c)));
        public string Details => string.Join(", ", new string[] { Name, Surname, Mobile, Email, }.Where(c => !string.IsNullOrWhiteSpace(c)));

        public string Photo => @"C:\InService\Attachments\farmers\";

        public decimal Balance => Payments.Select(c => c.Amount).DefaultIfEmpty(0).Sum() - UserDeductions.Select(c => c.Amount).DefaultIfEmpty(0).Sum();


        public IFarmer IFarmer => new IFarmer
        {
            Address = Address,
            City = City,
            DateOfBirth = DateOfBirth,
            District = District?.Name ?? "",
            Email = Email,
            Farmname = FarmName,
            Firstname = Name,
            GenderID = GenderID,
            ID = ID,
            IsSynced = true,
            Location = Location,
            Mobile = Mobile,
            NationalID = IDNumber,
            Province = Province?.Name ?? "",
            RegistrationDate = CreationDate,
            Surname = Surname,
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
