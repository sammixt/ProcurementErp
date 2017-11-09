using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErpVendor.Models
{
    public class MessageRecipientViewModel
    {
        public decimal ID { get; set; }
        public Nullable<decimal> RECIPIENT_ID { get; set; }
        public Nullable<decimal> RECIPIENT_GROUP_ID { get; set; }
        public string RECIPIENT_TYPE { get; set; }
        public Nullable<decimal> MESSAGE_ID { get; set; }
        public Nullable<decimal> IS_READ { get; set; }
    }
}