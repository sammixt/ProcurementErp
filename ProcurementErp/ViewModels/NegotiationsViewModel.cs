using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class NegotiationsViewModel
    {
        public NegotiationsViewModel()
        {
            this.NegItems = new List<RFQITEMViewModel>();    
        }
        public decimal NEG_NO { get; set; }
        public Nullable<decimal> RESPONSE_NO { get; set; }
        public string NEGOTIATOR { get; set; }
        public Nullable<decimal> TEMP_ID { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
        public Nullable<System.DateTime> RES_DATE { get; set; }
        public Nullable<decimal> TOTAL_AMT { get; set; }
        public Nullable<decimal> VAT { get; set; }
        public Nullable<decimal> VATVALUE { get; set; }
        public Nullable<decimal> GRANDTOTAL { get; set; }
        public string STATUS { get; set; }

        public List<RFQITEMViewModel> NegItems { get; set; }
    }
}