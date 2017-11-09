using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.Models
{
    public class ContactedVendorViewModel
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
        public Nullable<decimal> VAT { get; set; }
        public Nullable<decimal> TOTALBFTAX { get; set; }
        public Nullable<decimal> GRANDTOTAL { get; set; }
        public Nullable<decimal> EXECTOTAL { get; set; }
        public Nullable<decimal> EXECGRANDTOTAL { get; set; }
        public string CONTACT_NAME { get; set; }
        public string STATUS { get; set; }
        public Nullable<decimal> EXECVAT { get; set; }
        public Nullable<decimal> TOTALBFTAX_PROPOSAL { get; set; }
        public Nullable<decimal> VAT_PROPOSAL { get; set; }
        public Nullable<decimal> GRANDTOTAL_PROPOSAL { get; set; }
        public string NEG_STATUS { get; set; }
        public Nullable<System.DateTime> VERDICT_ISSUE_DATE { get; set; }
        public Nullable<System.DateTime> VERDICT_ACCEPT_DATE { get; set; }
    }

    public class QuotationViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
        public Nullable<decimal> ITEM_NO { get; set; }
        public Nullable<decimal> UNIT_PRICE { get; set; }
        public Nullable<decimal> TOTAL_PRICE { get; set; }
    }

    public class LorVendorViewModel
    {
        public string InitiatorName { get; set; }
        public string RefNum { get; set; }
        public string VendorName { get; set; }
    }
}