using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IFarmer
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Farmname { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NationalID { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public int GenderID { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsSynced { get; set; }
    }
}
