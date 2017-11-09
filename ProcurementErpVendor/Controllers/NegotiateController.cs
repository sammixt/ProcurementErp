using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ProcurementErpVendor.BLL;
using ProcurementErpVendor.Models;

namespace ProcurementErpVendor.Controllers
{
    public class NegotiateController : Controller
    {
        // GET: Negotiate
        public ActionResult NegotiatePerVendor(string TempNo, string NegNum = null)
        {
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            var Items = NegotiateBLL.GetItems(TempNo);
            TempData["TempNo"] = TempNo;
            TempData["VendorId"] = VendorId;
            ViewBag.TempNo = TempNo;
            ViewBag.NegNum = (NegNum != null) ? NegNum : "0";
            return PartialView(Items);
        }

        [HttpPost]
        public ActionResult Negotiate(RFQITEMViewModel[] inputs, FormCollection collections)
        {
            List<RFQITEMViewModel> _inputs = inputs.ToList();
            var principal = (ClaimsIdentity)User.Identity;
            string VendorId = principal.FindFirst(ClaimTypes.UserData).Value;
            decimal _VendorId = Convert.ToDecimal(VendorId);
            decimal? ReqType;
            string status;
            var TempNo = TempData["TempNo"] != null ? TempData["TempNo"].ToString() : null;
            var _TempNo = collections["TempNo"].ToString();
            decimal DTempNo = Convert.ToDecimal(_TempNo);
            ReqType = NegotiateBLL.getRequestType(DTempNo);

            status = NegotiateBLL.InsertNegotiationForRequest(_inputs, collections, _VendorId);
            if (status == "error")
            {
                TempData["ErrorMessage"] = "An Error Occurred";
            }
            else
            {
                TempData["SuccessMessage"] = "Negotiation Sent to selected vendors";
            }
            if (ReqType == 2)
            {

                return RedirectToAction("RFPSummary", "Request", new { Temp = TempNo});
                
            }
            else
            {
                return RedirectToAction("RFQSummary", "Request", new { id = TempNo});
            }
        }

        public ActionResult UpdateNegStatus(string TempNo, string NegNum = null, string Status = null)
        {
            var outcome = NegotiateBLL.UpdateNegStatus(TempNo, NegNum, Status);
            return Json(outcome, JsonRequestBehavior.AllowGet);
        }
    }
}