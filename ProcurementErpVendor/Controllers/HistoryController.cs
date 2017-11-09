using ProcurementErpVendor.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErpVendor.Controllers
{
    public class HistoryController : Controller
    {
        // GET: History
        public ActionResult RFPList()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetRFPRequsetHistory(VendorID);
            return View("~/Views/Request/RFPList.cshtml",_request);
        }

        public ActionResult RFQList()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetRFQRequsetHistory(VendorID);
            return View("~/Views/Request/RFQList.cshtml", _request);
        }

        public ActionResult RFIList()
        {
            var principal = (ClaimsIdentity)User.Identity;
            string _VendorID = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal VendorID = Convert.ToDecimal(_VendorID);
            var _request = GetRequest.GetRFIRequsetHistory(VendorID);
            return View("~/Views/Request/RFIList.cshtml", _request);
        }
    }
}