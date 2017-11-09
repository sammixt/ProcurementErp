using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class QuotationViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
        public Nullable<decimal> ITEM_NO { get; set; }
        public Nullable<decimal> UNIT_PRICE { get; set; }
        public Nullable<decimal> TOTAL_PRICE { get; set; }

        public ContactedVendorViewModel _VendorDetails { get; set; }
    }
}