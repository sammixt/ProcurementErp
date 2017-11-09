using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class VendorLoginViewModel
    {
        public decimal ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public Nullable<System.DateTime> LASTLOGIN { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
    }
}