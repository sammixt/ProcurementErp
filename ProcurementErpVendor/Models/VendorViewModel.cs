using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.Models
{
    public class VendorViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
        public Nullable<decimal> TEMP_NO { get; set; }
        public string VENDOR_NAME { get; set; }
        public Nullable<System.DateTime> RESPONSE_DATE { get; set; }
        public string AUTO_EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE { get; set; }
        public Nullable<System.DateTime> EXPECTED_DELIVERY_DATE { get; set; }
        public string PAYMENT_TERMS { get; set; }
        public string SUPPLIERS_QUOTE { get; set; }
        public string EMAIL { get; set; }
    }
}