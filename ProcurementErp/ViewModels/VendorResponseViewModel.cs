using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class VendorResponseViewModel
    {
        public VendorResponseViewModel()
        {
            this.RequestType = new RequestTypeViewModel();
            this.RefNum = new SOURCING_REF_TEMP_LINK();
            this.ContactedVendor = new List<ContactedVendorViewModel>();
        }

        public RequestTypeViewModel RequestType { get; set; }

        public SOURCING_REF_TEMP_LINK RefNum { get; set; }

        public List<ContactedVendorViewModel> ContactedVendor { get; set; }
    }
}