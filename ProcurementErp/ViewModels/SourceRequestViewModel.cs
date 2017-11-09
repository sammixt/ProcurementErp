using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class SourceRequestViewModel
    {
        public decimal TEMP_ID { get; set; }

        [Required(ErrorMessage = "Please select a Category from by clicking on the button above")]
        public string ITEM_CATEGORY { get; set; }
        public string INITIATOR_NAME { get; set; }
        public string INITIATOR_NUMBER { get; set; }

        [Display(Name = "Initiating Branch")]
        public string INITIATING_BRANCH { get; set; }
        public string INITIATING_BRANCHCODE { get; set; }

        [Display(Name = "Initiating Department")]
        public string INITIATING_DEPT { get; set; }
        public string INITIATING_DEPTCODE { get; set; }
        public Nullable<System.DateTime> INITIATION_DATE { get; set; }
        public string LINE_MANAGER_NUM { get; set; }
        public string LINE_MANAGER_NAME { get; set; }
        public string LINE_MANAGER_APPR { get; set; }
        public Nullable<System.DateTime> LINE_MANAGER_APPR_DATE { get; set; }
        public string BUYER_STATUS { get; set; }
        public string BUYER_COMMENT { get; set; }
        public string PREFERED_VENDOR { get; set; }
        public string PREFERED_VENDOR_ID { get; set; }
        public string INITIATOR_EMAIL { get; set; }
        public string LINE_MANAGER_EMAIL { get; set; }
        public string ITEM_CATEGORY_ID { get; set; }

        public Nullable<decimal> MAPTOREQUEST { get; set; }

        public string PROCUREMENT_BUYER { get; set; }
        public Nullable<decimal> PROCUREMENT_BUYER_ID { get; set; }

        [Required(ErrorMessage = "Please enter a description for the item")]
        [Display(Name = "Scope of Requirement/Item Description")]
        public string ITEM_DESCRIPTION { get; set; }

        [Display(Name = "Line Manger's Comment")]
        public string LINE_MANAGER_COMMENT { get; set; }

        [Display(Name = "Expected Delivery Date")]
        public Nullable<System.DateTime> EXPECTED_DELIVERY_DATE { get; set; }

        [Display(Name = "Upload Supporting File")]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }

        public IEnumerable<SOURCING_REQUEST_FILES> _Files { get; set; }
    }

    

}