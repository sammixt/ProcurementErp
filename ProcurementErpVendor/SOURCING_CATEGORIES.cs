//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProcurementErpVendor
{
    using System;
    using System.Collections.Generic;
    
    public partial class SOURCING_CATEGORIES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SOURCING_CATEGORIES()
        {
            this.SOURCING_USERS = new HashSet<SOURCING_USERS>();
        }
    
        public decimal CATEGORY_NUM { get; set; }
        public string CATEGORY_NAME { get; set; }
        public Nullable<decimal> CATEGORY_PARENT { get; set; }
        public string CATEGORY_STATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOURCING_USERS> SOURCING_USERS { get; set; }
    }
}
