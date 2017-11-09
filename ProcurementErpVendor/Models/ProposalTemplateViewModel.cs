using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.Models
{
    public class ProposalTemplateViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> VENDORID { get; set; }
        public Nullable<decimal> TEMPID { get; set; }
        [AllowHtml]
        public string PROPOSAL { get; set; }
        public Nullable<decimal> REQUEST_TYPE { get; set; }
    }
}