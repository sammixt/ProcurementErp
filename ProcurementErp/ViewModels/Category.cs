using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class Category
    {  
        public decimal CATEGORY_NUM { get; set; }
        public string CATEGORY_NAME { get; set; }
        public Nullable<decimal> CATEGORY_PARENT { get; set; }
        public string CATEGORY_STATE { get; set; }
    }
}