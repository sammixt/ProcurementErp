using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.ViewModels
{
    public class RFQITEMViewModel
    {
        public decimal ID { get; set; }
        public decimal ITEM_NO { get; set; }
        public Nullable<decimal> TEMP_NO { get; set; }
        public string QUANTITY { get; set; }
        public string UNIT_OF_MEAS { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<decimal> UNIT_PRICE { get; set; }
        public Nullable<decimal> TOTAL_PRICE { get; set; }
        public decimal NegNum { get; set; }

        [AllowHtml]
        public string RFI_INFO { get; set; }
    }
}