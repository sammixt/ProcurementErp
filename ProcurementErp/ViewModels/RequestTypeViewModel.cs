using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace ProcurementErp.ViewModels
{
    public class RequestTypeViewModel
    {
        public decimal REQUEST_ID { get; set; }
        [Display(Name = "SELECT REQUEST TYPE")]
        public string REQUEST_NAME { get; set; }
        public string REQUEST_SHORTCODE { get; set; }

        public List<SelectListItem> _Request { get; set; }
    }
}