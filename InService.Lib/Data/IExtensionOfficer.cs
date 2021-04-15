using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IExtensionOfficer
    {
        [PrimaryKey]
        public System.Guid ID { get; set; }
        public string ECNumber { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string NationalID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int? UserID { get; set; }
        public bool IsSynced { get; set; }
        public int? ErrorCode { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public int? GenderID { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
