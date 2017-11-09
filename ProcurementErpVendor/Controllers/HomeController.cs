using ProcurementErpVendor.BLL;
using ProcurementErpVendor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        // GET: Home
        public ActionResult Index()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetAllRequest(VendorID);
            return View(_request);
        }

        
    }
}