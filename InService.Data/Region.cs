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
    
    public partial class Region
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Region()
        {
            this.Countries = new HashSet<Country>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int ContinentID { get; set; }
        public int CreatorID { get; set; }
        public System.DateTime CreationDate { get; set; }
    
        public virtual Continent Continent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Country> Countries { get; set; }
        public virtual User User { get; set; }
    }
}
