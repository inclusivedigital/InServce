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
    
    public partial class NonValueChain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NonValueChain()
        {
            this.Courses = new HashSet<Course>();
            this.NonValueChain1 = new HashSet<NonValueChain>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int CreatorID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> BranchID { get; set; }
    
        public virtual Branch Branch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NonValueChain> NonValueChain1 { get; set; }
        public virtual NonValueChain NonValueChain2 { get; set; }
        public virtual User User { get; set; }
    }
}