using ProcurementErp.BLL;
using ProcurementErp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProcurementErp.Controllers
{
    public class RunningRequestController : Controller
    {
        // GET: RunningRequest
        public ActionResult RFIList()
        {
            List<RunningRequestViewModel> _req = new List<RunningRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            decimal _UserId = Convert.ToDecimal(UserId);
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            RunningRequest _getRequest = new RunningRequest(_UserId, null, 1);
            if (UserRole == "Administrator")
            {
                _req = _getRequest.GetRFIRequsetAdmin();
            }
            else
            {
                _req = _getRequest.GetRFIRequset(); ;
            }
            ViewBag.RequestName = "Request for Information";
            ViewBag.RequestShortName = "RFI";
            return View("RunningRequest",_req);
        }

        public ActionResult RFPList()
        {
            List<RunningRequestViewModel> _req = new List<RunningRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            decimal _UserId = Convert.ToDecimal(UserId);
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            RunningRequest _getRequest = new RunningRequest(_UserId, null, 2);
            if (UserRole == "Administrator")
            {
                _req = _getRequest.GetRFIRequsetAdmin();
            }
            else
            {
                _req = _getRequest.GetRFIRequset(); ;
            }
            ViewBag.RequestName = "Request for Proposal";
            ViewBag.RequestShortName = "RFP";
            return View("RunningRequest", _req);
        }

        public ActionResult RFQList()
        {
            List<RunningRequestViewModel> _req = new List<RunningRequestViewModel>();
            var principal = (ClaimsIdentity)User.Identity;
            string UserId = principal.FindFirst(ClaimTypes.Actor).Value;
            decimal _UserId = Convert.ToDecimal(UserId);
            string UserRole = principal.FindFirst(ClaimTypes.Role).Value;
            RunningRequest _getRequest = new RunningRequest(_UserId, null, 3);
            if (UserRole == "Administrator")
            {
                _req = _getRequest.GetRFQRequsetAdmin();
            }
            else
            {
                _req = _getRequest.GetRFQRequset(); ;
            }
            ViewBag.RequestName = "Request for Quotation";
            ViewBag.RequestShortName = "RFQ";
            return View("RunningRequest", _req);
        }
    }
}