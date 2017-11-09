using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class RFIRFPREQViewModel
    {
        public decimal TEMP_ID { get; set; }

        [Required]
        public string PRJECT_TITLE { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ISSUE_DATE { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> DUE_DATE { get; set; }
        public string DUE_TIME { get; set; }
        public string UBN_OVERVIEW { get; set; }
        public string PROJECT_OBJECTIVE { get; set; }
        public string WORK_SCOPE { get; set; }
        public string TECHNICAL_PROPOSAL { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> LST_QRY_RECEIPT_DATE { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> BANK_QRY_RES_DATE { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> LST_RPF_RECPT_DATE { get; set; }
        public Nullable<decimal> INITIATOR { get; set; }
        public string INITIATOR_NAME { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> INITIATION_DATE { get; set; }
        public string STATUS { get; set; }
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

    public class RFIRFPComboViewModel
    {
        public RFIRFPComboViewModel()
        {
            this.Request = new RFIRFPREQViewModel();
            this.Commercial = new ContactPersonViewModel();
            this.Technical = new TechnicalContactPerson();
            this.Vendors = new List<ContactedVendorViewModel>();
            this.Item = new List<RFQITEMViewModel>();
            this.RefNum = new SOURCING_REF_TEMP_LINK();
            this.proposal = new ProposalTemplateViewModel();
            this.Vendor = new ContactedVendorViewModel();
            this._Negotiations = new List<NegotiationsViewModel>();
        }
        public RFIRFPREQViewModel Request { get; set; }

        public ContactPersonViewModel Commercial { get; set; }

        public TechnicalContactPerson Technical { get; set; }

        public List<ContactedVendorViewModel> Vendors { get; set; }

        public List<RFQITEMViewModel> Item { get; set; }

        public SOURCING_REF_TEMP_LINK RefNum { get; set; }

        public IEnumerable<SOURCING_REQUEST_FILES> _Files { get; set; }

        public ProposalTemplateViewModel proposal { get; set; }

        public ContactedVendorViewModel Vendor { get; set; }

        public List<NegotiationsViewModel> _Negotiations { get; set; }
    }
}

