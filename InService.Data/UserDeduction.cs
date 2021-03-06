//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InService.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDeduction
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyID { get; set; }
        public Nullable<int> PaymentMethodID { get; set; }
        public Nullable<int> ExaminationID { get; set; }
        public Nullable<int> CourseID { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public Nullable<System.Guid> FarmerID { get; set; }
        public int CreatorID { get; set; }
        public System.DateTime CreationDate { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Examination Examination { get; set; }
        public virtual Farmer Farmer { get; set; }
        public virtual Module Module { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual User User { get; set; }
    }
}
