using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class AnalysisViewModel : RFQITEMViewModel
    {
        public List<QuotationViewModel> _VendorsQuote { get; set; }
    }

}