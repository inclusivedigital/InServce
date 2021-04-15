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
    
    public partial class CropCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CropCategory()
        {
            this.Crops = new HashSet<Crop>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<System.Guid> IconID { get; set; }
    
        public virtual Attachment Attachment { get; set; }
        public virtual Branch Branch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Crop> Crops { get; set; }
        public virtual User User { get; set; }
    }
}
