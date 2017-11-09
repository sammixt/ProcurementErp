using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProcurementErp.ViewModels
{
    public class RolesViewModel
    {
        public decimal ROLE_ID { get; set; }

        [Required(ErrorMessage = "Enter a role name")]
        [Display(Name = "Role Name")]
        public string ROLE_NAME { get; set; }

        [Display(Name = "Role Status")]
        public string STATUS { get; set; }
    }
}