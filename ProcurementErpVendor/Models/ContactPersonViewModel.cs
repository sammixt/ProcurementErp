using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.Models
{
    public class ContactPersonViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> TEMP_NO { get; set; }
        public string NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string TELEPHONE { get; set; }
        public string CONTACT_TYPE { get; set; }
        public string EMAIL { get; set; }
    }

    public class TechnicalContactPerson : ContactPersonViewModel
    {

    }
}