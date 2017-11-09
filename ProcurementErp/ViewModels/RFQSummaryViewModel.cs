using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class RFQSummaryViewModel
    {
        public RFQSummaryViewModel()
        {
            this.Request = new RFQREQViewModel();
            this.Initiator = new UserViewModel();
            this.Items = new List<RFQITEMViewModel>();
            this.Vendors = new List<ContactedVendorViewModel>();
            this.RefNum = new SOURCING_REF_TEMP_LINK();
        }

        public RFQREQViewModel Request { get; set; }

        public UserViewModel Initiator { get; set; }

        public SOURCING_REF_TEMP_LINK RefNum { get; set; }

        public List<RFQITEMViewModel> Items { get; set; }

        public List<ContactedVendorViewModel> Vendors { get; set; }

    }

    public class RFQSummaryDisplay : RFQSummaryViewModel
    {

        public RFQSummaryDisplay()
        {
            this.Request = new RFQREQViewModel();
            this.Initiator = new UserViewModel();
            this.Items = new List<RFQITEMViewModel>();
            this.RefNum = new SOURCING_REF_TEMP_LINK();
            this._Vendor = new ContactedVendorViewModel();
            this._Negotiations = new List<NegotiationsViewModel>();
        }
        public ContactedVendorViewModel _Vendor { get; set; }
        public List<NegotiationsViewModel> _Negotiations { get; set; }
        public IEnumerable<SOURCING_REQUEST_FILES> _Files { get; set; }

    }
}