using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class RFQREQViewModel 
    {   
        public decimal TEMP_NO { get; set; }
        public Nullable<System.DateTime> RFQ_START_DATE { get; set; }
        public Nullable<System.DateTime> RFQ_CLOSE_DATE { get; set; }
        public Nullable<decimal> INITIATOR_ID { get; set; }
        public string INITIATOR_NAME { get; set; }
        public string DELIVERY_ADDRESS { get; set; }
        public Nullable<decimal> NEGTOTALAMT { get; set; }
        public Nullable<decimal> NEGVAT { get; set; }
        public Nullable<decimal> NEGVATVALUE { get; set; }
        public Nullable<decimal> NEGGRANDTOTAL { get; set; }
        public Nullable<decimal> EXECTOTALAMT { get; set; }
        public Nullable<decimal> EXECVAT { get; set; }
        public Nullable<decimal> EXECVATVALUE { get; set; }
        public Nullable<decimal> EXECGRANDTOTAL { get; set; }
        public Nullable<decimal> MAPPING { get; set; }
        
    }
}