using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class VendorViewModel
    {
        public string VENDOR_CODE {get; set;}
        public decimal VENDOR_ID {get; set;}
        public string VENDOR_NUMBER {get; set;}
        public string VENDOR_NAME {get; set;}
        public string ENABLED_FLAG {get; set;}
        public string ADDRESS_LINE1 {get; set;}
        public string ADDRESS_LINE2 {get; set;}
        public string CITY {get; set;}
        public string STATE {get; set;}
        public string PERSON_FIRST_NAME {get; set;}
        public string PERSON_LAST_NAME {get; set;}
        public string PHONE_NUMBER {get; set;}
        public string EMAIL_ADDRESS { get; set; }
    }
}