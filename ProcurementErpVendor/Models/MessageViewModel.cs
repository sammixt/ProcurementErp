using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.Models
{
    public class MessageViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> TEMP_ID { get; set; }
        public Nullable<decimal> CREATORID { get; set; }
        public string CREATORTYPE { get; set; }
        public string MAIL_SUBJECT { get; set; }
        public Nullable<decimal> PARENT_MESSAGE_ID { get; set; }
        public Nullable<System.DateTime> DATE_CREATED { get; set; }
        public Nullable<decimal> INITIALIZER { get; set; }
        [AllowHtml]
        public string MESSAGE_BODY { get; set; }
    }
}