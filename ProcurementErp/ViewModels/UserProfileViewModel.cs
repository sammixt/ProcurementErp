using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EmployeeNumber { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string BranchCode { get; set; }

        public string Branch { get; set; }

        public string Department { get; set; }

        public string IsActive { get; set; }

        public string CostCenterCode { get; set; }

        public string SupervisorsId { get; set; }

        public string MobileNumber { get; set; }

        public string JobTitle { get; set; }

        public string TelephoneNumber { get; set; }

        public ICollection<string> Roles { get; set; }
        //for retrieving users account information
        public string CustId { get; set; }
    }

    public class UserViewModel
    {
        public decimal USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        [Display(Name="Employee Number")]
        public string EMP_NUM { get; set; }
        public string NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string TELEPHONE { get; set; }
        public string CATEGORY { get; set; }
        public string ROLE { get; set; }
        public string STATUS { get; set; }
        public List<SelectListItem> _Roles { get; set; }
        public List<SelectListItem> _Category { get; set; }
        [Display(Name = "Category")]
        public Nullable<decimal> CATEGORY_ID { get; set; }
        [Display(Name = "Role")]
        public Nullable<decimal> ROLE_ID { get; set; }
    }

     public class DeptCodeViewModel
    {
        public string DeptCostCode { get; set; }

        public string DeptName { get; set; }
    }

}